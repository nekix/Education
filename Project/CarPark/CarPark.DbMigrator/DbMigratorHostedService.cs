using Microsoft.Extensions.Hosting;

namespace CarPark.DbMigrator;

internal sealed class DbMigratorHostedService : IHostedService
{
    private readonly CarParkDbMigrationService _dbMigrationService;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public DbMigratorHostedService(CarParkDbMigrationService dbMigrationService, 
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _dbMigrationService = dbMigrationService;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dbMigrationService.MigrateAsync();

        _hostApplicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}