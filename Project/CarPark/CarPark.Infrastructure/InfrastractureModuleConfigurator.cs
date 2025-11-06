using CarPark.Data;
using CarPark.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CarPark;

public class InfrastractureModuleConfigurator
{
    public void ConfigureModule(IServiceCollection services, IConfiguration configuration, Action<InfrastractureModuleOptions>? configure)
    {
        OptionsBuilder<InfrastractureModuleOptions> optBuilder = services
            .AddOptions<InfrastractureModuleOptions>()
            .Bind(configuration);

        if (configure != null)
            optBuilder.Configure(configure);

        optBuilder.ValidateDataAnnotations()
            .ValidateOnStart();

        //string connectionString = configuration.GetConnectionString("Default")
        //                          ?? throw new InvalidOperationException("Unable to find database connection string. 'ConnectionStrings:Default' must be defined in configuration.");

        //// Add Timezone=UTC to connection string to prevent PostgreSQL from converting timestamptz to local timezone
        //if (!connectionString.Contains("Timezone=", StringComparison.OrdinalIgnoreCase))
        //{
        //    connectionString += ";Timezone=UTC";
        //}

        services.AddDbContextFactory<ApplicationDbContext>((provider, builder) =>
        {
            string connectionStr = provider.GetRequiredService<IOptions<InfrastractureModuleOptions>>()
                .Value
                .ConnectionString;

            builder.UseNpgsql(connectionStr, o => o.UseNetTopologySuite())
                .UseSnakeCaseNamingConvention();
        });

        //services.AddDbContext<ApplicationDbContext>(options => 
        //    options.UseNpgsql(connectionString, o => o.UseNetTopologySuite())
        //        .UseSnakeCaseNamingConvention());

        services.AddScoped<IVehiclesDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IModelsDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IEnterprisesDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IVehicleGeoTimePointsDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IRidesDbSet, ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }
}