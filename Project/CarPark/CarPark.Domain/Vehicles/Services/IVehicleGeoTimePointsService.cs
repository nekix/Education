using FluentResults;

namespace CarPark.Vehicles.Services;

public interface IVehicleGeoTimePointsService
{
    Result<VehicleGeoTimePoint> CreateVehicleGeoTimePoint(CreateVehicleGeoTimePointRequest request);
    Result UpdateVehicleGeoTimePoint(VehicleGeoTimePoint point, UpdateVehicleGeoTimePointRequest request);
}