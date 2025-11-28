using CarPark.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data.Interfaces;

public interface IVehicleGeoTimePointsDbSet
{
    DbSet<VehicleGeoTimePoint> VehicleGeoTimePoints { get; }
}