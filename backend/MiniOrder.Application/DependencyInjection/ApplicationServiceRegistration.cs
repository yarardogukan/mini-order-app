using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace MiniOrder.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            typeof(ApplicationServiceRegistration).Assembly);

        return services;
    }
}