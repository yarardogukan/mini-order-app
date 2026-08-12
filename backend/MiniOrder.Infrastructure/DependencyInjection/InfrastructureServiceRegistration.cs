using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniOrder.Application.Interfaces;
using MiniOrder.Infrastructure.Persistence;
using MiniOrder.Infrastructure.Services;

namespace MiniOrder.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<MiniOrderDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddMemoryCache();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();

        return services;
    }
}
