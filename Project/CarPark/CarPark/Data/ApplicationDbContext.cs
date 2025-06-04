using CarPark.Models;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>()
            .ToTable("vehicle");

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Id)
            .UseIdentityAlwaysColumn();
    }
}