using Microsoft.Extensions.Logging;

namespace CarPark.DbMigrator;

internal class CarParkDbMigrationService
{
    private readonly ILogger<CarParkDbMigrationService> _logger;
    private readonly EntityFrameworkCoreMyProjectNameDbSchemaMigrator _dbSchemaMigrator;

    public CarParkDbMigrationService(ILogger<CarParkDbMigrationService> logger, 
        EntityFrameworkCoreMyProjectNameDbSchemaMigrator dbSchemaMigrator)
    {
        _logger = logger;
        _dbSchemaMigrator = dbSchemaMigrator;
    }

    public async Task MigrateAsync()
    {
        await MigrateDatabaseSchemaAsync();
    }

    private async Task MigrateDatabaseSchemaAsync()
    {
        _logger.LogInformation("Started database migrations...");

        await _dbSchemaMigrator.MigrateAsync();

        _logger.LogInformation($"Successfully completed host database migrations.");
    }
}