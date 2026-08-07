using System.Linq.Expressions;
using MiniOrder.Application.DTOs.Orders.Responses;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Mapping;

internal static class OrderMappings
{
    public static Expression<Func<Order, OrderResponse>> ToResponse()
    {
        return order => new OrderResponse(
            order.Id,
            order.CustomerName,
            order.CreatedAt,
            order.TotalAmount,
            order.Items
                .OrderBy(item => item.Id)
                .Select(item => new OrderItemResponse(
                    item.ProductId,
                    item.Product.Name,
                    item.Product.StockCode,
                    item.Quantity,
                    item.UnitPrice,
                    item.LineTotal))
                .ToList());
    }
}