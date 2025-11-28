using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace CarPark.DbMigrator;

internal sealed class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("CarPark", LogEventLevel.Warning)
            #if DEBUG
            .MinimumLevel.Override("CarPark.DbMigrator", LogEventLevel.Debug)
            #else
            .MinimumLevel.Override("CarPark.DbMigrator", LogEventLevel.Information)
            #endif
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        await CreateHostBuilder(args).RunConsoleAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(config => config.AddUserSecrets<Program>())
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            })
            .ConfigureServices((hostContext, services) =>
            {
                DbMigratorModuleConfiguration dbMigratorModuleConfiguration = new DbMigratorModuleConfiguration();
                dbMigratorModuleConfiguration.ConfigureModule(services, hostContext.Configuration);

                DbMigratorOptions dbMigratorOptions = hostContext.Configuration.GetRequiredSection(DbMigratorOptions.Key)
                    .Get<DbMigratorOptions>() 
                    ?? throw new ArgumentException($"Configuration section '{DbMigratorOptions.Key}' not founded");

                InfrastractureModuleConfigurator infrastractureModule = new InfrastractureModuleConfigurator();
                infrastractureModule.ConfigureModule(services, hostContext.Configuration, options =>
                {
                    options.ConnectionString = dbMigratorOptions.ConnectionString;
                });
            });
}