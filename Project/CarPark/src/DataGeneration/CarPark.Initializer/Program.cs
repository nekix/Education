using CarPark.Initializer.Demo;
using CarPark.Initializer.Minimal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.CommandLine;

namespace CarPark.Initializer;

internal class Program
{
    static async Task<int> Main(string[] args)
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

        RootCommand rootCommand = new RootCommand("Утилита инициализации данных CarPark");

        Command generateCommand = new Command("init", "Инициализация данных");

        generateCommand.Add(CreateInitMinimalCommand());
        generateCommand.Add(CreateInitDemoCommand());

        rootCommand.Add(generateCommand);

        return await rootCommand.Parse(args).InvokeAsync();
    }

    private static Command CreateInitMinimalCommand()
    {
        Option<string> connectionStringOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };

        Command command = new Command("minimal", "Генерация мимнимального набора данных")
        {
            connectionStringOption
        };

        command.SetAction(async parseResult =>
        {
            MinimalInitializerModuleOptions options = new MinimalInitializerModuleOptions
            {
                ConnectionString = parseResult.GetRequiredValue<string>(connectionStringOption)
            };

            await CreateMinimalInitializeHostBuilder(options)
                .RunConsoleAsync();
        });

        return command;
    }

    private static Command CreateInitDemoCommand()
    {
        Option<string> connectionStringOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };
        Option<string> graphHopperApiKeyOption = new Option<string>("--graphhopper-api-key", "-ghk") { Description = "API ключ GraphHopper", Required = true };

        Command command = new Command("demo", "Генерация демонстрационного набора данных")
        {
            connectionStringOption,
            graphHopperApiKeyOption
        };

        command.SetAction(async parseResult =>
        {
            DemoInitializerModuleOptions options = new DemoInitializerModuleOptions
            {
                ConnectionString = parseResult.GetRequiredValue<string>(connectionStringOption),
                GraphHopperApiKey = parseResult.GetRequiredValue<string>(graphHopperApiKeyOption)
            };

            await CreateDemoInitializeHostBuilder(options)
                .RunConsoleAsync();
        });

        return command;
    }

    private static IHostBuilder CreateBaseHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(config => config.AddUserSecrets<Program>())
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            });

    private static IHostBuilder CreateMinimalInitializeHostBuilder(MinimalInitializerModuleOptions moduleOpt) =>
        CreateBaseHostBuilder() 
            .ConfigureServices((hostContext, services) =>
            { 
                services.AddOptions<MinimalInitializerModuleOptions>()
                    .Configure(opt =>
                    {
                        opt.ConnectionString = moduleOpt.ConnectionString;
                    })
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

                InfrastractureModuleConfigurator infrastractureModule = new InfrastractureModuleConfigurator();
                infrastractureModule.ConfigureModule(services, hostContext.Configuration, options =>
                {
                    options.ConnectionString = moduleOpt.ConnectionString;
                });

                services.AddHostedService<MinimalInitializerHostedService>();
            });

    private static IHostBuilder CreateDemoInitializeHostBuilder(DemoInitializerModuleOptions moduleOpt) =>
        CreateBaseHostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddOptions<DemoInitializerModuleOptions>()
                    .Configure(opt =>
                    {
                        opt.ConnectionString = moduleOpt.ConnectionString;
                        opt.GraphHopperApiKey = moduleOpt.GraphHopperApiKey;
                    })
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

                InfrastractureModuleConfigurator infrastractureModule = new InfrastractureModuleConfigurator();
                infrastractureModule.ConfigureModule(services, hostContext.Configuration, options =>
                {
                    options.ConnectionString = moduleOpt.ConnectionString;
                });

                services.AddHostedService<DemoInitializerHostedService>();
            });
}