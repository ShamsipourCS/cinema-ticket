using CinemaTicket.API.Middleware;
using CinemaTicket.Application;
using CinemaTicket.Infrastructure;
using CinemaTicket.Persistence;
using CinemaTicket.Persistence.Context;
using CinemaTicket.Persistence.Seeders;
using Microsoft.AspNetCore.Authorization;
using Serilog;

// Configure Serilog bootstrap logger for startup errors
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting CinemaTicket API...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Validate required configuration on startup
    ValidateConfiguration(builder.Configuration);

    // Configure Serilog from configuration
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Add services to the container.
    builder.Services.AddHostedService<CinemaTicket.Infrastructure.BackgroundJobs.ReservationCleanupService>();
    builder.Services.AddControllers();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IAuthorizationHandler, CinemaTicket.API.Authorization.TicketOwnershipHandler>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
        };
    });

    // Configure authorization policies
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Authenticated", policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy("CustomerAccess", policy =>
            policy.RequireRole("Customer", "Admin"));

        // Resource-based ownership validation
        options.AddPolicy("TicketOwner", policy =>
            policy.Requirements.Add(new CinemaTicket.API.Authorization.TicketOwnershipRequirement()));
    });

    var app = builder.Build();

    // Apply database migrations and seed data in Development
    if (app.Environment.IsDevelopment())
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Starting database seeding...");
                await DatabaseSeeder.SeedAsync(context, services);
                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }

    // Configure the HTTP request pipeline.

    // Exception Handling Middleware (must be FIRST in pipeline)
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Serilog request logging with enrichment
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());

            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
            }
        };
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("CinemaTicket API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "CinemaTicket API terminated unexpectedly");
    throw;
}
finally
{
    Log.Information("CinemaTicket API shutting down...");
    Log.CloseAndFlush();
}

static void ValidateConfiguration(IConfiguration configuration)
{
    var requiredSettings = new Dictionary<string, string>
    {
        ["JwtSettings:Secret"] = "JWT secret key",
        ["JwtSettings:Issuer"] = "JWT issuer",
        ["JwtSettings:Audience"] = "JWT audience",
        ["ConnectionStrings:DefaultConnection"] = "Database connection string"
    };

    var missingSettings = requiredSettings
        .Where(s => string.IsNullOrEmpty(configuration[s.Key]))
        .Select(s => s.Value)
        .ToList();

    if (missingSettings.Any())
    {
        throw new InvalidOperationException(
            $"Missing required configuration: {string.Join(", ", missingSettings)}. " +
            "Copy appsettings.Development.sample.json to appsettings.Development.json and configure secrets.");
    }

    var jwtSecret = configuration["JwtSettings:Secret"];
    if (jwtSecret!.Length < 32)
    {
        throw new InvalidOperationException(
            "JwtSettings:Secret must be at least 32 characters for security.");
    }
}
