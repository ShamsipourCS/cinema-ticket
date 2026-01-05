using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaTicket.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure-related services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        return services;
    }
}
