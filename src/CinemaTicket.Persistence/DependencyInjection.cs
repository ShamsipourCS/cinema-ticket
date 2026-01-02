using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Persistence.Context;
using CinemaTicket.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaTicket.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register Unit of Work - this provides access to all repositories
        // ApplicationDbContext implements IUnitOfWork and creates repositories internally
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Note: Individual repositories are NOT registered in DI container
        // They are created by ApplicationDbContext as needed (lazy initialization)
        // Access repositories through IUnitOfWork.Users, IUnitOfWork.Movies, etc.

        return services;
    }
}
