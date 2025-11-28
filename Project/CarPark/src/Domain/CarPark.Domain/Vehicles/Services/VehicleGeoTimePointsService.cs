using FluentResults;

namespace CarPark.Vehicles.Services;

public class VehicleGeoTimePointsService : IVehicleGeoTimePointsService
{
    public Result<VehicleGeoTimePoint> CreateVehicleGeoTimePoint(CreateVehicleGeoTimePointRequest request)
    {
        VehicleGeoTimePointCreateData data = new VehicleGeoTimePointCreateData
        {
            Id = request.Id,
            Vehicle = request.Vehicle,
            Location = request.Location,
            Time = request.Time
        };

        return VehicleGeoTimePoint.Create(data);
    }

    public Result UpdateVehicleGeoTimePoint(VehicleGeoTimePoint point, UpdateVehicleGeoTimePointRequest request)
    {
        VehicleGeoTimePointUpdateData data = new VehicleGeoTimePointUpdateData
        {
            Vehicle = request.Vehicle,
            Location = request.Location,
            Time = request.Time
        };

        return VehicleGeoTimePoint.Update(point, data);
    }
}