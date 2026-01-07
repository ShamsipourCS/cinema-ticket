using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaTicket.Application;

/// <summary>
/// Dependency injection configuration for the Application layer.
/// Registers MediatR, pipeline behaviors, and FluentValidation.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR from executing assembly
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Register pipeline behaviors (ORDER MATTERS!)
        // 1. ValidationBehavior runs first - validates requests before processing
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.ValidationBehavior<,>));

        // 2. LoggingBehavior runs second - logs request execution and timing
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.LoggingBehavior<,>));

        // Register FluentValidation validators from assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register HTTP context accessor for getting authenticated user in handlers
        services.AddHttpContextAccessor();

        return services;
    }
}
