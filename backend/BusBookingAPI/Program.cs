
using BusBookingAPI.Data;
using BusBookingAPI.Services;
using BusBookingAPI.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;

namespace BusBookingAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog for detailed logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/app-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 30)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "BusBookingAPI")
            .CreateLogger();

        builder.Host.UseSerilog();

        // Add services to the container.

        // Database configuration
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "root";
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "busBooking";
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";

        var finalConnectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

        builder.Services.AddDbContext<BusBookingDbContext>(options =>
            options.UseNpgsql(finalConnectionString));

        // Configure global DateTime handling for PostgreSQL
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });
        builder.Services.AddAuthorization();

        // JWT Configuration
        var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-secret-key-change-this-in-production-at-least-32-characters-long";
        var key = Encoding.ASCII.GetBytes(jwtSecret);

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "BusBookingAPI",
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"] ?? "BusBookingClient",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        // Register services
        builder.Services.AddScoped<IBusService, BusService>();
        builder.Services.AddScoped<IBookingService, BookingService>();
        builder.Services.AddScoped<IBusAvailabilityService, BusAvailabilityService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IOperatorAuthService, OperatorAuthService>();
        builder.Services.AddScoped<ILocationService, LocationService>();
        builder.Services.AddScoped<IStateService, StateService>();
        builder.Services.AddScoped<IDistrictService, DistrictService>();
        builder.Services.AddScoped<ICountryService, CountryService>();
        builder.Services.AddScoped<IRouteService, RouteService>();
        builder.Services.AddScoped<IOperatorService, OperatorService>();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // Add Swagger/OpenAPI
        builder.Services.AddSwaggerGen();

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bus Booking API v1");
                options.RoutePrefix = string.Empty; // Swagger UI at root
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");

        // Add request/response logging middleware
        app.UseMiddleware<RequestResponseLoggingMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
