using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniOrder.Application.Common.Results;
using MiniOrder.Application.Common.Errors.BusinessErrors;
using MiniOrder.Application.Common.Errors;
using MiniOrder.Application.DTOs.Orders.Requests;
using MiniOrder.Application.DTOs.Orders.Responses;
using MiniOrder.Application.Interfaces;
using MiniOrder.Infrastructure.Persistence;
using FluentValidation;
using MiniOrder.Infrastructure.Mapping;
using MiniOrder.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace MiniOrder.Infrastructure.Services;

public sealed class OrderService : IOrderService
{
    private readonly MiniOrderDbContext _dbContext;
    private readonly ILogger<OrderService> _logger;
    private readonly IMemoryCache _cache;
    private readonly IValidator<CreateOrderRequest> _createOrderValidator;

    public OrderService(
    MiniOrderDbContext dbContext,
    ILogger<OrderService> logger,
    IValidator<CreateOrderRequest> createOrderValidator,
    IMemoryCache cache)
    {
        _dbContext = dbContext;
        _logger = logger;
        _createOrderValidator = createOrderValidator;
        _cache = cache;
    }

    #region Commands

    public async Task<Result<OrderResponse>> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _createOrderValidator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var validationMessage = string.Join(
                " ",
                validationResult.Errors.Select(error => error.ErrorMessage));

            _logger.LogWarning(
                "Order creation validation failed. Errors: {ValidationErrors}",
                validationMessage);

            return Result<OrderResponse>.Failure(
                new Error(
                    "Order.ValidationFailed",
                    validationMessage));
        }

        var productIds = request.Items
            .Select(item => item.ProductId)
            .ToList();

        var products = await _dbContext.Products
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Count)
        {
            var missingProductId = productIds
                .Except(products.Select(product => product.Id))
                .First();

            _logger.LogWarning(
                "Order creation failed. Product not found. ProductId: {ProductId}",
                missingProductId);

            return Result<OrderResponse>.Failure(
                ProductErrors.NotFound(missingProductId));
        }

        foreach (var item in request.Items)
        {
            var product = products.First(
                product => product.Id == item.ProductId);

            if (product.StockQuantity < item.Quantity)
            {
                _logger.LogWarning(
                    "Order creation failed. Insufficient stock. ProductId: {ProductId}",
                    product.Id);

                return Result<OrderResponse>.Failure(
                    ProductErrors.InsufficientStock(
                        product.Name,
                        item.Quantity,
                        product.StockQuantity));
            }
        }

        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            var order = BuildOrder(request, products);

            await _dbContext.Orders.AddAsync(
                order,
                cancellationToken);

            await _dbContext.SaveChangesAsync(
      cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            foreach (var productId in productIds)
            {
                _cache.Remove($"product:{productId}");
            }

            _logger.LogInformation(
                "Order created successfully. OrderId: {OrderId}, CustomerName: {CustomerName}, TotalAmount: {TotalAmount}",
                order.Id,
                order.CustomerName,
                order.TotalAmount);

            _logger.LogInformation(
                "Order created successfully. OrderId: {OrderId}, CustomerName: {CustomerName}, TotalAmount: {TotalAmount}",
                order.Id,
                order.CustomerName,
                order.TotalAmount);

            var response = CreateResponse(order, products);

            return Result<OrderResponse>.Success(response);
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _logger.LogError(
                exception,
                "Order creation failed unexpectedly. CustomerName: {CustomerName}",
                request.CustomerName);

            throw;
        }
    }

    #endregion

    #region Queries

    public async Task<Result<IReadOnlyCollection<OrderResponse>>> GetAllAsync(
    CancellationToken cancellationToken = default)
    {
        var orders = await _dbContext.Orders
            .AsNoTracking()
            .OrderByDescending(order => order.Id)
            .Select(OrderMappings.ToResponse())
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "Orders retrieved. Count: {Count}",
            orders.Count);

        return Result<IReadOnlyCollection<OrderResponse>>
            .Success(orders);
    }

    public async Task<Result<OrderResponse>> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.Orders
            .AsNoTracking()
            .Where(order => order.Id == id)
            .Select(OrderMappings.ToResponse())
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            _logger.LogWarning(
                "Order not found. OrderId: {OrderId}",
                id);

            return Result<OrderResponse>.Failure(
                OrderErrors.NotFound(id));
        }

        _logger.LogInformation(
            "Order retrieved successfully. OrderId: {OrderId}",
            id);

        return Result<OrderResponse>.Success(order);
    }

    #endregion

    #region Helpers

    private static Order BuildOrder(
        CreateOrderRequest request,
        IReadOnlyCollection<Product> products)
    {
        var order = new Order
        {
            CustomerName = request.CustomerName.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        foreach (var item in request.Items)
        {
            var product = products.First(
                product => product.Id == item.ProductId);

            var lineTotal = product.Price * item.Quantity;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                LineTotal = lineTotal
            });

            product.StockQuantity -= item.Quantity;
        }

        order.TotalAmount = order.Items.Sum(
            item => item.LineTotal);

        return order;
    }

    private static OrderResponse CreateResponse(
        Order order,
        IReadOnlyCollection<Product> products)
    {
        return new OrderResponse(
            order.Id,
            order.CustomerName,
            order.CreatedAt,
            order.TotalAmount,
            order.Items
                .OrderBy(item => item.Id)
                .Select(item =>
                {
                    var product = products.First(
                        product => product.Id == item.ProductId);

                    return new OrderItemResponse(
                        item.ProductId,
                        product.Name,
                        product.StockCode,
                        item.Quantity,
                        item.UnitPrice,
                        item.LineTotal);
                })
                .ToList());
    }

    #endregion
}