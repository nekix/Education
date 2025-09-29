namespace CarPark.Vehicles;

public interface IVehiclesDataSource
{
    public IQueryable<Vehicle> Vehicles { get; }

    public IQueryable<VehicleGeoTimePoint> VehicleGeoTimePoints { get; }
}