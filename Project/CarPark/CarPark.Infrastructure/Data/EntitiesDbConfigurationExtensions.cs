using System.Security.Cryptography.X509Certificates;
using CarPark.Drivers;
using CarPark.Enterprises;
using CarPark.Managers;
using CarPark.Models;
using CarPark.Rides;
using CarPark.TimeZones;
using CarPark.Vehicles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data;

public static class EntitiesDbConfigurationExtensions
{
    public static void ConfigureModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Model>()
            .ToTable("model");

        modelBuilder.Entity<Model>()
            .HasKey(m => m.Id);
    }

    public static void ConfigureVehicle(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>()
            .ToTable("vehicle");

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Id)
            .UseIdentityAlwaysColumn();

        modelBuilder.Entity<Vehicle>()
            .HasOne<Model>(m => m.Model)
            .WithMany()
            .HasPrincipalKey(m => m.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }

    public static void ConfigureDriver(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Driver>()
            .ToTable("driver");

        modelBuilder.Entity<Driver>()
            .Property(d => d.Id);

        modelBuilder.Entity<Driver>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Driver>()
            .HasMany(d => d.AssignedVehicles)
            .WithMany(v => v.AssignedDrivers)
            .UsingEntity("driver_vehicle_assignment");

        modelBuilder.Entity<Driver>()
            .HasMany(d => d.AssignedVehicles)
            .WithMany(v => v.AssignedDrivers)
            .UsingEntity("driver_vehicle_assignment");

        modelBuilder.Entity<Driver>()
            .HasOne(d => d.ActiveAssignedVehicle)
            .WithOne(v => v.ActiveAssignedDriver)
            .HasForeignKey<Driver>("assigned_vehicle_id")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }

    public static void ConfigureTzInfo(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TzInfo>()
            .ToTable("time_zone");

        modelBuilder.Entity<TzInfo>()
            .Property(tz => tz.Id)
            .UseIdentityColumn();
    }

    public static void ConfigureEnterprise(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enterprise>()
            .ToTable("enterprise");

        modelBuilder.Entity<Enterprise>()
            .Property(e => e.Id)
            .UseIdentityAlwaysColumn();

        modelBuilder.Entity<Enterprise>()
            .HasMany<Vehicle>()
            .WithOne(v => v.Enterprise)
            .HasConstraintName("fk_vehicle_enterprise_enterprise_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Enterprise>()
            .HasMany<Driver>()
            .WithOne()
            .HasForeignKey(d => d.EnterpriseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Enterprise>()
            .HasMany(e => e.Managers)
            .WithMany(m => m.Enterprises)
            .UsingEntity("enterprise_manager");;
    }

    public static void ConfigureManager(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>()
            .ToTable("manager");

        modelBuilder.Entity<Manager>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<Manager>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(m => m.IdentityUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }

    public static void ConfigureVehicleGeoTimePoint(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VehicleGeoTimePoint>()
            .ToTable("vehicle_geo_time_point");

        modelBuilder.Entity<VehicleGeoTimePoint>()
            .Property(g => g.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<VehicleGeoTimePoint>()
            .Property(g => g.Location)
            .HasColumnType("geometry (point, 4326)");

        modelBuilder.Entity<VehicleGeoTimePoint>()
            .HasOne(p => p.Vehicle)
            .WithMany()
            .HasForeignKey("vehicle_id")
            .IsRequired();
    }

    public static void ConfigureRides(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ride>()
            .ToTable("ride");

        modelBuilder.Entity<Ride>()
            .Property(r => r.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Ride>()
            .HasOne(r => r.Vehicle)
            .WithMany()
            .HasForeignKey("vehicle_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Ride>()
            .HasOne(r => r.StartPoint)
            .WithMany()
            .HasForeignKey("start_geo_time_point_id")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Ride>()
            .HasOne(r => r.EndPoint)
            .WithMany()
            .HasForeignKey("end_geo_time_point_id")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}