using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Enums;
using CinemaTicket.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaTicket.Persistence.Seeders;

/// <summary>
/// Seeds initial user accounts with properly hashed passwords.
/// Creates 1 admin and 3 customer accounts for development/testing.
/// </summary>
public static class UserSeeder
{
    /// <summary>
    /// Seeds users with hashed passwords. This method is idempotent.
    /// </summary>
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        // Check if users already exist (idempotency)
        if (await context.Users.AnyAsync(cancellationToken))
            return;

        // Resolve password hasher from DI
        var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<User>>();

        var users = new List<User>
        {
            // Admin User
            new User
            {
                Id = new Guid("USER0001-0001-0001-0001-000000000001"),
                Email = "admin@cinematicket.com",
                FirstName = "System",
                LastName = "Administrator",
                Phone = "+98-21-1234-5678",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow.AddDays(-30) // Created 30 days ago
            },

            // Customer Users
            new User
            {
                Id = new Guid("USER0002-0001-0001-0001-000000000002"),
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                Phone = "+98-21-2345-6789",
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new User
            {
                Id = new Guid("USER0003-0001-0001-0001-000000000003"),
                Email = "jane.smith@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                Phone = "+98-21-3456-7890",
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new User
            {
                Id = new Guid("USER0004-0001-0001-0001-000000000004"),
                Email = "michael.brown@example.com",
                FirstName = "Michael",
                LastName = "Brown",
                Phone = "+98-21-4567-8901",
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        // Hash passwords for all users
        // Admin password: Admin123!
        // Customer password: Customer123!
        foreach (var user in users)
        {
            var password = user.Role == UserRole.Admin ? "Admin123!" : "Customer123!";
            user.PasswordHash = passwordHasher.HashPassword(user, password);
        }

        await context.Users.AddRangeAsync(users, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
