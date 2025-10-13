using CarPark.Data;
using CarPark.Data.Interfaces;
using CarPark.Shared.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarPark;

public class InfrastractureModuleConfigurator : IModuleConfigurator
{
    public void ConfigureModule(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Default")
                                  ?? throw new InvalidOperationException("Unable to find database connection string. 'ConnectionStrings:Default' must be defined in configuration.");

        // Add Timezone=UTC to connection string to prevent PostgreSQL from converting timestamptz to local timezone
        if (!connectionString.Contains("Timezone=", StringComparison.OrdinalIgnoreCase))
        {
            connectionString += ";Timezone=UTC";
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, o => o.UseNetTopologySuite())
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IVehiclesDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IModelsDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IEnterprisesDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IVehicleGeoTimePointsDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IRidesDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }
}