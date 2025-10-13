using CarPark.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CarPark;

public class MigrationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        string connectionString = args[0];

        // Add Timezone=UTC to connection string to prevent PostgreSQL from converting timestamptz to local timezone
        if (!connectionString.Contains("Timezone=", StringComparison.OrdinalIgnoreCase))
        {
            connectionString += ";Timezone=UTC";
        }

        optionsBuilder.UseNpgsql(args[0], o => o.UseNetTopologySuite())
            .UseSnakeCaseNamingConvention();

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}