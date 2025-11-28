using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarPark.DbMigrator;

internal sealed class DbMigratorModuleConfiguration
{
    public void ConfigureModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DbMigratorOptions>()
            .Bind(configuration.GetSection(DbMigratorOptions.Key))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<EntityFrameworkCoreMyProjectNameDbSchemaMigrator>();

        services.AddTransient<CarParkDbMigrationService>();

        services.AddHostedService<DbMigratorHostedService>();
    }
}