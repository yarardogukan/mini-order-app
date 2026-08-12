using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniOrder.Application.Common.Errors;
using MiniOrder.Application.Common.Results;
using MiniOrder.Application.DTOs.Carts.Requests;
using MiniOrder.Application.DTOs.Carts.Responses;
using MiniOrder.Application.Interfaces;
using MiniOrder.Domain.Entities;
using MiniOrder.Infrastructure.Persistence;

namespace MiniOrder.Infrastructure.Services;

public sealed class CartService : ICartService
{
    private readonly MiniOrderDbContext _dbContext;
    private readonly ILogger<CartService> _logger;
    private readonly IValidator<AddCartItemRequest> _addItemValidator;
    private readonly IValidator<UpdateCartItemRequest> _updateItemValidator;

    public CartService(
        MiniOrderDbContext dbContext,
        ILogger<CartService> logger,
        IValidator<AddCartItemRequest> addItemValidator,
        IValidator<UpdateCartItemRequest> updateItemValidator
    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _addItemValidator = addItemValidator;
        _updateItemValidator = updateItemValidator;
    }

    #region Queries

    public async Task<Result<CartResponse>> GetCartAsync(
        Guid cartId,
        CancellationToken cancellationToken = default
    )
    {
        var cart = await _dbContext
            .Carts.AsNoTracking()
            .Include(cart => cart.Items)
                .ThenInclude(cartItem => cartItem.Product)
                    .ThenInclude(product => product.Brand)
            .Include(cart => cart.Items)
                .ThenInclude(cartItem => cartItem.Product)
                    .ThenInclude(product => product.Category)
            .Include(cart => cart.Items)
                .ThenInclude(cartItem => cartItem.Product)
                    .ThenInclude(product => product.Images)
            .FirstOrDefaultAsync(cart => cart.Id == cartId, cancellationToken);

        if (cart is null)
        {
            _logger.LogWarning("Cart not found. CartId: {CartId}", cartId);

            return Result<CartResponse>.Failure(
                new Error("Cart.NotFound", $"Cart with id '{cartId}' was not found.")
            );
        }

        var items = cart
            .Items.OrderBy(cartItem => cartItem.Id)
            .Select(cartItem =>
            {
                var product = cartItem.Product;

                var coverImageUrl = product
                    .Images.OrderByDescending(image => image.IsCover)
                    .ThenBy(image => image.SortOrder)
                    .Select(image => image.ImageUrl)
                    .FirstOrDefault();

                return new CartItemResponse
                {
                    ProductId = product.Id,
                    StockCode = product.StockCode,
                    ProductName = product.Name,
                    BrandName = product.Brand.Name,
                    CategoryName = product.Category.Name,
                    CoverImageUrl = coverImageUrl,
                    UnitPrice = product.Price,
                    Quantity = cartItem.Quantity,
                    LineTotal = product.Price * cartItem.Quantity,
                };
            })
            .ToList();

        var itemCount = items.Sum(item => item.Quantity);
        var subtotal = items.Sum(item => item.LineTotal);

        var response = new CartResponse
        {
            CartId = cart.Id,
            ItemCount = itemCount,
            Subtotal = subtotal,
            Total = subtotal,
            Items = items,
        };

        _logger.LogInformation(
            "Cart retrieved. CartId: {CartId}, ItemCount: {ItemCount}, Subtotal: {Subtotal}",
            cart.Id,
            response.ItemCount,
            response.Subtotal
        );

        return Result<CartResponse>.Success(response);
    }

    #endregion

    #region Commands

    public async Task<Result<CartResponse>> AddItemAsync(
        Guid? cartId,
        AddCartItemRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var validationResult = await _addItemValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(
                " ",
                validationResult.Errors.Select(error => error.ErrorMessage)
            );

            return Result<CartResponse>.Failure(new Error("Cart.ValidationFailed", errorMessage));
        }

        var product = await _dbContext
            .Products.Include(product => product.Category)
            .Include(product => product.Brand)
            .FirstOrDefaultAsync(
                product =>
                    product.Id == request.ProductId
                    && product.IsActive
                    && product.Category.IsActive
                    && product.Brand.IsActive,
                cancellationToken
            );

        if (product is null)
        {
            return Result<CartResponse>.Failure(
                new Error(
                    "Cart.ProductNotFound",
                    $"Product with id '{request.ProductId}' was not found or is unavailable."
                )
            );
        }

        Cart? cart = null;

        if (cartId.HasValue)
        {
            cart = await _dbContext
                .Carts.Include(cart => cart.Items)
                .FirstOrDefaultAsync(cart => cart.Id == cartId.Value, cancellationToken);

            if (cart is null)
            {
                return Result<CartResponse>.Failure(
                    new Error("Cart.NotFound", $"Cart with id '{cartId.Value}' was not found.")
                );
            }
        }
        else
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            await _dbContext.Carts.AddAsync(cart, cancellationToken);
        }

        var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == request.ProductId);

        var requestedQuantity = existingItem is null
            ? request.Quantity
            : existingItem.Quantity + request.Quantity;

        if (requestedQuantity > product.StockQuantity)
        {
            return Result<CartResponse>.Failure(
                new Error(
                    "Cart.InsufficientStock",
                    $"Requested quantity exceeds available stock for product '{product.Name}'."
                )
            );
        }

        if (existingItem is null)
        {
            cart.Items.Add(new CartItem { ProductId = product.Id, Quantity = request.Quantity });
        }
        else
        {
            existingItem.Quantity = requestedQuantity;
        }

        cart.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Cart item added. CartId: {CartId}, ProductId: {ProductId}, Quantity: {Quantity}",
            cart.Id,
            product.Id,
            request.Quantity
        );

        return await GetCartAsync(cart.Id, cancellationToken);
    }

    public async Task<Result<CartResponse>> UpdateItemQuantityAsync(
        Guid cartId,
        int productId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var validationResult = await _updateItemValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(
                " ",
                validationResult.Errors.Select(error => error.ErrorMessage)
            );

            return Result<CartResponse>.Failure(new Error("Cart.ValidationFailed", errorMessage));
        }

        var cart = await _dbContext
            .Carts.Include(cart => cart.Items)
                .ThenInclude(cartItem => cartItem.Product)
                    .ThenInclude(product => product.Category)
            .Include(cart => cart.Items)
                .ThenInclude(cartItem => cartItem.Product)
                    .ThenInclude(product => product.Brand)
            .FirstOrDefaultAsync(cart => cart.Id == cartId, cancellationToken);

        if (cart is null)
        {
            _logger.LogWarning(
                "Cart not found while updating item quantity. CartId: {CartId}",
                cartId
            );

            return Result<CartResponse>.Failure(
                new Error("Cart.NotFound", $"Cart with id '{cartId}' was not found.")
            );
        }

        var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);

        if (cartItem is null)
        {
            _logger.LogWarning(
                "Cart item not found. CartId: {CartId}, ProductId: {ProductId}",
                cartId,
                productId
            );

            return Result<CartResponse>.Failure(
                new Error(
                    "Cart.ItemNotFound",
                    $"Product with id '{productId}' was not found in the cart."
                )
            );
        }

        var product = cartItem.Product;

        if (!product.IsActive || !product.Category.IsActive || !product.Brand.IsActive)
        {
            return Result<CartResponse>.Failure(
                new Error(
                    "Cart.ProductUnavailable",
                    $"Product with id '{productId}' is unavailable."
                )
            );
        }

        if (request.Quantity > product.StockQuantity)
        {
            return Result<CartResponse>.Failure(
                new Error(
                    "Cart.InsufficientStock",
                    $"Requested quantity exceeds available stock for product '{product.Name}'."
                )
            );
        }

        cartItem.Quantity = request.Quantity;
        cart.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Cart item quantity updated. CartId: {CartId}, ProductId: {ProductId}, Quantity: {Quantity}",
            cart.Id,
            productId,
            request.Quantity
        );

        return await GetCartAsync(cart.Id, cancellationToken);
    }

    public async Task<Result<CartResponse>> RemoveItemAsync(
        Guid cartId,
        int productId,
        CancellationToken cancellationToken = default
    )
    {
        var cart = await _dbContext
            .Carts.Include(cart => cart.Items)
            .FirstOrDefaultAsync(cart => cart.Id == cartId, cancellationToken);

        if (cart is null)
        {
            _logger.LogWarning("Cart not found while removing item. CartId: {CartId}", cartId);

            return Result<CartResponse>.Failure(
                new Error("Cart.NotFound", $"Cart with id '{cartId}' was not found.")
            );
        }

        var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);

        if (cartItem is null)
        {
            _logger.LogWarning(
                "Cart item not found while removing. CartId: {CartId}, ProductId: {ProductId}",
                cartId,
                productId
            );

            return Result<CartResponse>.Failure(
                new Error(
                    "Cart.ItemNotFound",
                    $"Product with id '{productId}' was not found in the cart."
                )
            );
        }

        cart.Items.Remove(cartItem);

        cart.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Cart item removed. CartId: {CartId}, ProductId: {ProductId}",
            cart.Id,
            productId
        );

        return await GetCartAsync(cart.Id, cancellationToken);
    }

    public async Task<Result<bool>> ClearAsync(
        Guid cartId,
        CancellationToken cancellationToken = default
    )
    {
        var cart = await _dbContext
            .Carts.Include(cart => cart.Items)
            .FirstOrDefaultAsync(cart => cart.Id == cartId, cancellationToken);

        if (cart is null)
        {
            _logger.LogWarning("Cart not found while clearing. CartId: {CartId}", cartId);

            return Result<bool>.Failure(
                new Error("Cart.NotFound", $"Cart with id '{cartId}' was not found.")
            );
        }

        if (cart.Items.Count == 0)
        {
            _logger.LogInformation("Cart already empty. CartId: {CartId}", cart.Id);

            return Result<bool>.Success(true);
        }

        cart.Items.Clear();
        cart.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cart cleared. CartId: {CartId}", cart.Id);

        return Result<bool>.Success(true);
    }

    #endregion
}
