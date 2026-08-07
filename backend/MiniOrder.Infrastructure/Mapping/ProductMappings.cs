using System.Linq.Expressions;
using MiniOrder.Application.DTOs.Products.Responses;
using MiniOrder.Domain.Entities;

namespace MiniOrder.Infrastructure.Mapping;

internal static class ProductMappings
{
    public static Expression<Func<Product, ProductResponse>> ToResponse()
    {
        return product => new ProductResponse(
            product.Id,
            product.StockCode,
            product.Name,
            product.Price,
            product.StockQuantity);
    }
}