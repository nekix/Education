using CarPark.Models.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data;

public interface IVehiclesDbSet : IDisposable
{
    public DbSet<Vehicle> Vehicles { get; }
}