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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSeeding((context, _) =>
        {
            if (!context.Set<Vehicle>().Any())
            {
                context.Set<Vehicle>().AddRange(
                    new Vehicle { VinNumber = "VIN0001", Price = 10000, ManufactureYear = 2015, Mileage = 50000, Color = "Красный" },
                    new Vehicle { VinNumber = "VIN0002", Price = 12000, ManufactureYear = 2017, Mileage = 30000, Color = "Синий" },
                    new Vehicle { VinNumber = "VIN0003", Price = 9000, ManufactureYear = 2013, Mileage = 70000, Color = "Чёрный" },
                    new Vehicle { VinNumber = "VIN0004", Price = 15000, ManufactureYear = 2019, Mileage = 20000, Color = "Белый" },
                    new Vehicle { VinNumber = "VIN0005", Price = 11000, ManufactureYear = 2016, Mileage = 45000, Color = "Зелёный" },
                    new Vehicle { VinNumber = "VIN0006", Price = 13000, ManufactureYear = 2018, Mileage = 35000, Color = "Серебристый" },
                    new Vehicle { VinNumber = "VIN0007", Price = 9500, ManufactureYear = 2014, Mileage = 60000, Color = "Серый" },
                    new Vehicle { VinNumber = "VIN0008", Price = 14000, ManufactureYear = 2020, Mileage = 15000, Color = "Бежевый" },
                    new Vehicle { VinNumber = "VIN0009", Price = 12500, ManufactureYear = 2017, Mileage = 40000, Color = "Коричневый" },
                    new Vehicle { VinNumber = "VIN0010", Price = 16000, ManufactureYear = 2021, Mileage = 10000, Color = "Фиолетовый" }
                );

                context.SaveChanges();
            }
        });

        optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            if (!await context.Set<Vehicle>().AnyAsync(cancellationToken))
            {
                await context.Set<Vehicle>().AddRangeAsync(
                    new Vehicle { VinNumber = "VIN0001", Price = 10000, ManufactureYear = 2015, Mileage = 50000, Color = "Красный" },
                    new Vehicle { VinNumber = "VIN0002", Price = 12000, ManufactureYear = 2017, Mileage = 30000, Color = "Синий" },
                    new Vehicle { VinNumber = "VIN0003", Price = 9000, ManufactureYear = 2013, Mileage = 70000, Color = "Чёрный" },
                    new Vehicle { VinNumber = "VIN0004", Price = 15000, ManufactureYear = 2019, Mileage = 20000, Color = "Белый" },
                    new Vehicle { VinNumber = "VIN0005", Price = 11000, ManufactureYear = 2016, Mileage = 45000, Color = "Зелёный" },
                    new Vehicle { VinNumber = "VIN0006", Price = 13000, ManufactureYear = 2018, Mileage = 35000, Color = "Серебристый" },
                    new Vehicle { VinNumber = "VIN0007", Price = 9500, ManufactureYear = 2014, Mileage = 60000, Color = "Серый" },
                    new Vehicle { VinNumber = "VIN0008", Price = 14000, ManufactureYear = 2020, Mileage = 15000, Color = "Бежевый" },
                    new Vehicle { VinNumber = "VIN0009", Price = 12500, ManufactureYear = 2017, Mileage = 40000, Color = "Коричневый" },
                    new Vehicle { VinNumber = "VIN0010", Price = 16000, ManufactureYear = 2021, Mileage = 10000, Color = "Фиолетовый" }
                );

                await context.SaveChangesAsync(cancellationToken);
            }
        });
    }
}