﻿using CarPark.Data;
using CarPark.Drivers;
using CarPark.Enterprises;
using CarPark.Managers;
using CarPark.ManagersOperations.ExportImport.Commands;
using CarPark.ManagersOperations.ExportImport.Queries;
using CarPark.Models;
using CarPark.Rides;
using CarPark.Shared.CQ;
using CarPark.Shared.DateTimes;
using CarPark.TimeZones;
using CarPark.Vehicles;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CarPark.ManagersOperations.ExportImport;

public class ManagersExportImportHandler : BaseManagersHandler,
    IQueryHandler<ExportEnterpriseQuery, Result<EnterpriseExportImportDto>>,
    IQueryHandler<ExportEnterpriseVehiclesQuery, Result<List<VehicleExportImportDto>>>,
    IQueryHandler<ExportVehicleRidesQuery, Result<List<VehicleRideExportImportDto>>>,
    IQueryHandler<ExportVehicleTrackQuery, Result<List<VehicleGeoTimePointExportImportDto>>>,
    IQueryHandler<ExportModelsQuery, Result<List<ModelExportImportDto>>>,
    ICommandHandler<ImportCommand, Result>
{
    public ManagersExportImportHandler(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Result<EnterpriseExportImportDto>> Handle(ExportEnterpriseQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<EnterpriseExportImportDto>();

        Manager manager = getManager.Value;

        Enterprise? enterprise = manager.Enterprises.FirstOrDefault(e => e.Id == query.EnterpriseId);

        if (enterprise == null)
            return Result.Fail<EnterpriseExportImportDto>(ExportImportHandlerErrors.ManagerNotAllowedToEnterprise);

        EnterpriseExportImportDto enterpriseInfo = 
            new EnterpriseExportImportDto
            {
                Id = enterprise.Id,
                Name = enterprise.Name,
                LegalAddress = enterprise.LegalAddress,
                IanaTzId = enterprise.TimeZone?.IanaTzId,
                WindowsTzId = enterprise.TimeZone?.WindowsTzId,
            };

        return Result.Ok(enterpriseInfo);
    }


    public async Task<Result<List<VehicleExportImportDto>>> Handle(ExportEnterpriseVehiclesQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<List<VehicleExportImportDto>>();

        Manager manager = getManager.Value;

        Enterprise? enterprise = manager.Enterprises.FirstOrDefault(e => e.Id == query.EnterpriseId);

        if (enterprise == null)
            return Result.Fail<List<VehicleExportImportDto>>(ExportImportHandlerErrors.ManagerNotAllowedToEnterprise);

        List<VehicleExportImportDto> vehicleDtos = await DbContext.Vehicles
            .Where(v => v.Enterprise.Id == query.EnterpriseId)
            .OrderBy(v => v.Id)
            .Select(v => new VehicleExportImportDto
            {
                Id = v.Id,
                ModelId = v.Model.Id,
                EnterpriseId = v.Enterprise.Id,
                VinNumber = v.VinNumber,
                Price = v.Price,
                ManufactureYear = v.ManufactureYear,
                Mileage = v.Mileage,
                Color = v.Color,
                AddedToEnterpriseAt = v.AddedToEnterpriseAt,
            })
            .ToListAsync();

        return Result.Ok(vehicleDtos);
    }

    public async Task<Result<List<VehicleRideExportImportDto>>> Handle(ExportVehicleRidesQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<List<VehicleRideExportImportDto>>();

        Manager manager = getManager.Value;

        Vehicle? vehicle = await DbContext.Vehicles
            .Include(v => v.Enterprise)
            .FirstOrDefaultAsync(v => v.Id == query.VehicleId);

        if (vehicle == null)
            return Result.Fail<List<VehicleRideExportImportDto>>(ExportImportHandlerErrors.VehicleNotExist);

        // Получаем только поездки
        List<VehicleRideExportImportDto> rides = await DbContext.Rides
            .Where(r => r.Vehicle.Id == vehicle.Id)
            .Where(r => r.StartTime >= query.RidesStartTime && r.EndTime <= query.RidesEndTime)
            .OrderBy(r => r.StartTime)
            .Select(ride => new VehicleRideExportImportDto
            {
                Id = ride.Id,
                VehicleId = ride.Vehicle.Id,
                StartTime = ride.StartTime,
                EndTime = ride.EndTime,
                StartVehicleGeoTimePointId = ride.StartPoint.Id,
                EndVehicleGeoTimePointId = ride.EndPoint.Id
            })
            .ToListAsync();

        return Result.Ok(rides);
    }

    public async Task<Result<List<VehicleGeoTimePointExportImportDto>>> Handle(ExportVehicleTrackQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<List<VehicleGeoTimePointExportImportDto>>();

        Manager manager = getManager.Value;

        Vehicle? vehicle = await DbContext.Vehicles
            .Include(v => v.Enterprise)
            .FirstOrDefaultAsync(v => v.Id == query.VehicleId);

        if (vehicle == null)
            return Result.Fail<List<VehicleGeoTimePointExportImportDto>>(ExportImportHandlerErrors.VehicleNotExist);

        if (manager.Enterprises.All(e => e.Id != vehicle.Enterprise.Id))
            return Result.Fail<List<VehicleGeoTimePointExportImportDto>>(ExportImportHandlerErrors.ManagerNotAllowedToEnterprise);

        // Получаем треки в заданном временном диапазоне
        List<VehicleGeoTimePointExportImportDto> tracks = await DbContext.VehicleGeoTimePoints
            .Where(p => p.Vehicle.Id == vehicle.Id)
            .Where(p => p.Time >= query.TrackStartTime && p.Time <= query.TrackEndTime)
            .OrderBy(p => p.Time)
            .Select(p => new VehicleGeoTimePointExportImportDto
            {
                Id = p.Id,
                VehicleId = p.Vehicle.Id,
                Time = p.Time,
                X = p.Location.X,
                Y = p.Location.Y
            })
            .ToListAsync();

        return Result.Ok(tracks);
    }

    public async Task<Result<List<ModelExportImportDto>>> Handle(ExportModelsQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<List<ModelExportImportDto>>();

        List<ModelExportImportDto> modelDtos = await DbContext.Models
            .OrderBy(m => m.Id)
            .Select(m => new ModelExportImportDto
            {
                Id = m.Id,
                ModelName = m.ModelName,
                VehicleType = m.VehicleType,
                SeatsCount = m.SeatsCount,
                MaxLoadingWeightKg = m.MaxLoadingWeightKg,
                EnginePowerKW = m.EnginePowerKW,
                TransmissionType = m.TransmissionType,
                FuelSystemType = m.FuelSystemType,
                FuelTankVolumeLiters = m.FuelTankVolumeLiters
            })
            .ToListAsync();

        return Result.Ok(modelDtos);
    }

    public async Task<Result> Handle(ImportCommand command)
    {
        Result<Manager> getManager = await GetManagerAsync(command.RequestingManagerId, true);
        if (getManager.IsFailed)
            return getManager.ToResult();

        Manager manager = getManager.Value;

        List<Model> newModels = new List<Model>();

        if (command.Models != null)
        {
            foreach (ModelExportImportDto modelDto in command.Models)
            {
                Model model = new Model
                {
                    Id = modelDto.Id,
                    ModelName = modelDto.ModelName,
                    VehicleType = modelDto.VehicleType,
                    SeatsCount = modelDto.SeatsCount,
                    MaxLoadingWeightKg = modelDto.MaxLoadingWeightKg,
                    EnginePowerKW = modelDto.EnginePowerKW,
                    TransmissionType = modelDto.TransmissionType,
                    FuelSystemType = modelDto.FuelSystemType,
                    FuelTankVolumeLiters = modelDto.FuelTankVolumeLiters
                };

                newModels.Add(model);

                DbContext.Models.Add(model);
            }
        }

        List<Enterprise> newEnterprises = new List<Enterprise>();

        if (command.Enterprises != null)
        {
            foreach (EnterpriseExportImportDto enterpriseDto in command.Enterprises)
            {
                TzInfo? tzInfo;

                if (enterpriseDto.IanaTzId != null || enterpriseDto.WindowsTzId != null)
                {
                    tzInfo = await DbContext.TzInfos
                        .FirstOrDefaultAsync(tz =>
                            tz.IanaTzId == enterpriseDto.IanaTzId &&
                            tz.WindowsTzId == enterpriseDto.WindowsTzId);

                    if (tzInfo == null)
                        return Result.Fail(ExportImportHandlerErrors.TimeZoneNotExist);
                }
                else
                {
                    tzInfo = null;
                }

                Enterprise enterprise = new Enterprise
                {
                    Id = enterpriseDto.Id,
                    Name = enterpriseDto.Name,
                    LegalAddress = enterpriseDto.LegalAddress,
                    TimeZone = tzInfo,
                    Managers = new List<Manager> { manager }
                };

                newEnterprises.Add(enterprise);

                DbContext.Enterprises.Add(enterprise);
            }
        }

        List<Vehicle> newVehicles = new List<Vehicle>();

        if (command.Vehicles != null)
        {
            foreach (VehicleExportImportDto vehicleDto in command.Vehicles)
            {
                Enterprise? enterprise = await DbContext.Enterprises.FindAsync(vehicleDto.EnterpriseId);
                if (enterprise == null)
                    return Result.Fail(ExportImportHandlerErrors.EnterpriseNotFound);

                if (manager.Enterprises.All(e => e.Id != enterprise.Id))
                    return Result.Fail(ExportImportHandlerErrors.ManagerNotAllowedToEnterprise);

                Model? model = await DbContext.Models.FindAsync(vehicleDto.ModelId);
                if (model == null)
                    return Result.Fail(ExportImportHandlerErrors.ModelNotFound);

                Result<Vehicle> createVehicle = Vehicle.Create(
                    vehicleDto.Id,
                    model,
                    enterprise,
                    vehicleDto.VinNumber,
                    vehicleDto.Price,
                    vehicleDto.ManufactureYear,
                    vehicleDto.Mileage,
                    vehicleDto.Color,
                    new List<Driver>(0),
                    null,
                    vehicleDto.AddedToEnterpriseAt);

                if (createVehicle.IsFailed)
                    return createVehicle.ToResult();

                Vehicle vehicle = createVehicle.Value;

                newVehicles.Add(vehicle);

                DbContext.Vehicles.Add(vehicle);
            }
        }
        

        List<VehicleGeoTimePoint> track = new List<VehicleGeoTimePoint>();

        if (command.Tracks != null)
        {
            foreach (VehicleGeoTimePointExportImportDto pointDto in command.Tracks)
            {
                Vehicle? vehicle = await DbContext.Vehicles
                    .Include(v => v.Enterprise)
                    .FirstOrDefaultAsync(v => v.Id == pointDto.VehicleId);

                if (vehicle == null)
                    return Result.Fail(ExportImportHandlerErrors.VehicleNotExist);

                if (manager.Enterprises.All(e => e.Id != vehicle.Enterprise.Id))
                    return Result.Fail(ExportImportHandlerErrors.ManagerNotAllowedToEnterprise);

                Result<VehicleGeoTimePoint> createPoint = VehicleGeoTimePoint.Create(
                    pointDto.Id,
                    vehicle,
                    new Point(new Coordinate(pointDto.X, pointDto.Y)) { SRID = 4326 },
                    new UtcDateTimeOffset(pointDto.Time));

                if (createPoint.IsFailed)
                    return createPoint.ToResult();

                VehicleGeoTimePoint point = createPoint.Value;

                track.Add(point);

                DbContext.VehicleGeoTimePoints.AddRange(point);
            }
        }

        List<Ride> rides = new List<Ride>();

        if (command.Rides != null)
        {
            foreach (VehicleRideExportImportDto rideDto in command.Rides)
            {
                Vehicle? vehicle = await DbContext.Vehicles
                    .Include(v => v.Enterprise)
                    .FirstOrDefaultAsync(v => v.Id == rideDto.VehicleId);

                if (vehicle == null)
                    return Result.Fail(ExportImportHandlerErrors.VehicleNotExist);

                if (manager.Enterprises.All(e => e.Id != vehicle.Enterprise.Id))
                    return Result.Fail(ExportImportHandlerErrors.ManagerNotAllowedToEnterprise);

                VehicleGeoTimePoint? startPoint = await DbContext.VehicleGeoTimePoints
                    .FindAsync(rideDto.StartVehicleGeoTimePointId);

                if (startPoint == null)
                    return Result.Fail(ExportImportHandlerErrors.RidePointNotFound);

                VehicleGeoTimePoint? endPoint = await DbContext.VehicleGeoTimePoints
                    .FindAsync(rideDto.EndVehicleGeoTimePointId);

                if (endPoint == null)
                    return Result.Fail(ExportImportHandlerErrors.RidePointNotFound);

                Ride ride = new Ride
                {
                    Id = rideDto.Id,
                    Vehicle = vehicle,
                    StartTime = rideDto.StartTime,
                    EndTime = rideDto.EndTime,
                    StartPoint = startPoint,
                    EndPoint = endPoint
                };

                rides.Add(ride);

                DbContext.Rides.Add(ride);
            }
        }

        await DbContext.SaveChangesAsync();

        return Result.Ok();
    }
}