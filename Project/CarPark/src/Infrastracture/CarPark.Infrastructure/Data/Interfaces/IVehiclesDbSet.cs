using CarPark.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data.Interfaces;

public interface IVehiclesDbSet : IDisposable
{
    public DbSet<Vehicle> Vehicles { get; }
}