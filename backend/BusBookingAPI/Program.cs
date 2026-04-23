
using BusBookingAPI.Data;
using BusBookingAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace BusBookingAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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

        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        // Register services
        builder.Services.AddScoped<IBusService, BusService>();
        builder.Services.AddScoped<ILocationService, LocationService>();
        builder.Services.AddScoped<IStateService, StateService>();
        builder.Services.AddScoped<IDistrictService, DistrictService>();
        builder.Services.AddScoped<ICountryService, CountryService>();
        builder.Services.AddScoped<IRouteService, RouteService>();

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

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
