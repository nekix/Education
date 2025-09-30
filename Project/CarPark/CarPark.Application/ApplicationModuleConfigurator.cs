using CarPark.ManagersOperations.Drivers.Queries;
using CarPark.ManagersOperations.Drivers.Queries.Models;
using CarPark.ManagersOperations.Enterprises.Commands;
using CarPark.ManagersOperations.Enterprises.Queries;
using CarPark.ManagersOperations.Models.Commands;
using CarPark.ManagersOperations.Rides.Queries;
using CarPark.ManagersOperations.Tracks.Queries;
using CarPark.ManagersOperations.Tracks.Queries.Models;
using CarPark.ManagersOperations.Vehicles.Commands;
using CarPark.ManagersOperations.Vehicles.Queries;
using CarPark.ManagersOperations.Vehicles.Queries.Models;
using CarPark.Services.GeoServices;
using CarPark.Services.TimeZones;
using CarPark.Shared.CQ;
using CarPark.Shared.Modules;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Features;

namespace CarPark;

public class ApplicationModuleConfigurator : IModuleConfigurator
{
    public void ConfigureModule(IServiceCollection services, IConfiguration configuration)
    {
        ConfigureGeoCodingServies(services, configuration);
        ConfigureTimezoneServices(services);
        ConfigureCommandQueriesHandlers(services);
    }

    private static void ConfigureCommandQueriesHandlers(IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateModelCommand, Result<int>>, CreateModelCommand.Handler>();
        services.AddScoped<ICommandHandler<UpdateModelCommand, Result<int>>, UpdateModelCommand.Handler>();
        services.AddScoped<ICommandHandler<DeleteModelCommand, Result>, DeleteModelCommand.Handler>();

        services.AddScoped<IQueryHandler<GetEnterpriseQuery, Result<EnterpriseDto>>, ManagersEnterprisesQueryHandler>();
        services.AddScoped<IQueryHandler<GetEnterprisesCollectionQuery, Result<List<EnterpriseDto>>>, ManagersEnterprisesQueryHandler>();
        services.AddScoped<ICommandHandler<DeleteEnterpriseCommand, Result>, ManagersEnterpriseCommandHandler>();

        services.AddScoped<IQueryHandler<GetVehicleQuery, Result<VehicleDto>>, ManagersVehiclesQueryHandler>();
        services.AddScoped<IQueryHandler<GetVehiclesListQuery, Result<PaginatedVehicles>>, ManagersVehiclesQueryHandler>();
        services.AddScoped<ICommandHandler<CreateVehicleCommand, Result<int>>, ManagersVehiclesCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateVehicleCommand, Result<int>>, ManagersVehiclesCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteVehicleCommand, Result>, ManagersVehiclesCommandHandler>();
        
        services.AddScoped<IQueryHandler<GetDriverQuery, Result<DriverDto>>, ManagersDriverQueryHandler>();
        services.AddScoped<IQueryHandler<GetDriversListQuery, Result<PaginatedDrivers>>, ManagersDriverQueryHandler>();

        services.AddScoped<IQueryHandler<GetRidesQuery, Result<RidesViewModel>>, RidesQueryHandler>();

        services.AddScoped<IQueryHandler<GetTrackQuery, Result<TrackViewModel>>, ManagersTrackQueryHandler>();
        services.AddScoped<IQueryHandler<GetTrackFeatureCollectionQuery, Result<FeatureCollection>>, ManagersTrackQueryHandler>();
        services.AddScoped<IQueryHandler<GetRidesTrackQuery, Result<TrackViewModel>>, ManagersTrackQueryHandler>();
        services.AddScoped<IQueryHandler<GetRidesTrackFeatureCollectionQuery, Result<FeatureCollection>>, ManagersTrackQueryHandler>();
    }

    private static void ConfigureGeoCodingServies(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DadataSettings>()
            .Bind(configuration.GetSection(DadataSettings.ConfigurationSectionName))
            .ValidateDataAnnotations();

        services.AddHttpClient<IGeoCodingService, DadataGeoCodingService>();

        services.AddTransient<IGeoCodingService, DadataGeoCodingService>();
    }

    private static void ConfigureTimezoneServices(IServiceCollection services)
    {
        services.AddSingleton<LocalIcuTimezoneService>();
        services.AddScoped<ITimeZoneConversionService, TimeZoneConversionService>();
    }
}