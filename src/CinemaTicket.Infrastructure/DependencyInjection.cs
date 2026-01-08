using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Infrastructure.Services;
using CinemaTicket.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaTicket.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure-related services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure settings using Options pattern
        services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));

        // Register services with scoped lifetime (per-request)
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IStripePaymentService, StripePaymentService>();

        // Password Hashing
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }
}
