using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaTicket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        return services;
    }
}
