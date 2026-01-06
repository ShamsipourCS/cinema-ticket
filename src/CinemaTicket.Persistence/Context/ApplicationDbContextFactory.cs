using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // بارگذاری تنظیمات از appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  // مسیر جاری پروژه را تنظیم می‌کند
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)  // بارگذاری appsettings.json
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        optionsBuilder.UseSqlServer(connectionString);  // استفاده از SQL Server برای دیتابیس

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
