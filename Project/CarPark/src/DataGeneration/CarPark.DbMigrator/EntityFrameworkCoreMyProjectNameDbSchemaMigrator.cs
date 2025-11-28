using Microsoft.Extensions.DependencyInjection;
using CarPark.Data;
using Microsoft.EntityFrameworkCore;

namespace CarPark.DbMigrator;

internal class EntityFrameworkCoreMyProjectNameDbSchemaMigrator
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreMyProjectNameDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the MyProjectNameDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<ApplicationDbContext>()
            .Database
            .MigrateAsync();
    }
}