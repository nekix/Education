using CarPark.Rides;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data.Interfaces;

public interface IRidesDbSet
{
    public DbSet<Ride> Rides { get; }
}