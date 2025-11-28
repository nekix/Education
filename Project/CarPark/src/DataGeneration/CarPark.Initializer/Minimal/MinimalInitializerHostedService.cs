using CarPark.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CarPark.Initializer.Minimal;

/// <summary>
/// Hosted service responsible for initializing minimal reference data (time zones and car models)
/// required for the application startup.
/// </summary>
internal class MinimalInitializerHostedService : IHostedService
{
    private readonly ApplicationDbContext _context;
    private readonly MinimalInitializerModuleOptions _options;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public MinimalInitializerHostedService(ApplicationDbContext dbContext,
        IOptions<MinimalInitializerModuleOptions> options,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _context = dbContext;
        _options = options.Value;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    /// <summary>
    /// Starts the initialization process asynchronously.
    /// Generates time zones and car models using DataGenerator Main function.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to stop the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await ProcessAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize minimal data: {ex.Message}");
            throw;
        }

        _hostApplicationLifetime.StopApplication();
    }

    /// <summary>
    /// Stops the hosted service.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A completed task.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ProcessAsync(CancellationToken token)
    {
        // Check if reference data is already populated
        bool hasTimeZones = await _context.TzInfos.AnyAsync(token);

        if (hasTimeZones)
        {
            Console.WriteLine("Reference data (time zones) is already populated. Skipping initialization.");
            return;
        }

        Console.WriteLine("Starting minimal data initialization (time zones) using DataGenerator...");

        // Use fixed seed for deterministic generation
        const int seed = 42;

        // Call DataGenerator Main function with seed-reference command
        int exitCode = await DataGenerator.Program.Main(new string[]
        {
            "generate",
            "seed-reference",
            "--seed",
            seed.ToString(),
            "--connection-string",
            _options.ConnectionString
        });

        if (exitCode != 0)
        {
            throw new Exception($"DataGenerator exited with code {exitCode}");
        }

        Console.WriteLine("Minimal data initialization (time zones) completed successfully.");
    }
}