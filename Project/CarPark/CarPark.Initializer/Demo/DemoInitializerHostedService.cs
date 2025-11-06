using CarPark.Data;
using CarPark.DataGenerator;
using CarPark.TrackGenerator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.IO;

namespace CarPark.Initializer.Demo;

internal class DemoInitializerHostedService : IHostedService
{
    private readonly ApplicationDbContext _context;
    private readonly DemoInitializerModuleOptions _options;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public DemoInitializerHostedService(ApplicationDbContext dbContext,
        IOptions<DemoInitializerModuleOptions> options,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _context = dbContext;
        _options = options.Value;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await ProcessAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize demo data: {ex.Message}");
            throw;
        }

        _hostApplicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ProcessAsync(CancellationToken token)
    {
        Console.WriteLine("Starting demo data initialization...");

        // Generate time zones if needed
        bool hasTimeZones = await _context.TzInfos.AnyAsync(token);

        if (!hasTimeZones)
        {
            Console.WriteLine("Generating time zones...");
            int exitCode = await DataGenerator.Program.Main(new string[]
            {
                "generate",
                "seed-reference",
                "--seed",
                "42",
                "--connection-string",
                _options.ConnectionString
            });

            if (exitCode != 0)
            {
                throw new Exception($"DataGenerator seed-reference exited with code {exitCode}");
            }
        }

        // Check if basic demo data exists
        bool hasEnterprises = await _context.Enterprises.AnyAsync(token);
        bool hasVehicles = await _context.Vehicles.AnyAsync(token);
        bool hasDrivers = await _context.Drivers.AnyAsync(token);

        if (!hasEnterprises || !hasVehicles || !hasDrivers)
        {
            // Create temp file for vehicle IDs
            string tempFile = Path.GetTempFileName();

            // Generate full demo data
            Console.WriteLine("Generating enterprises, vehicles, and drivers...");
            int exitCode2 = await DataGenerator.Program.Main(new string[]
            {
                "generate",
                "full-demo",
                "--seed",
                "42",
                "--enterprises",
                "3",
                "--vehicles-per-enterprise",
                "30",
                "--drivers-per-enterprise",
                "50",
                "--export-vehicle-ids",
                tempFile,
                "--connection-string",
                _options.ConnectionString
            });

            if (exitCode2 != 0)
            {
                throw new Exception($"DataGenerator full-demo exited with code {exitCode2}");
            }

            // Check if vehicle IDs file was created
            if (!File.Exists(tempFile))
            {
                throw new Exception("Vehicle IDs file was not created by DataGenerator");
            }

            // Clean up temp file - we don't need it anymore since generate-rides works without file
            File.Delete(tempFile);
        }
        else
        {
            Console.WriteLine("Basic demo data (enterprises, vehicles, drivers) already exists. Skipping generation.");
        }

        // Always try to generate tracks and rides if vehicles exist
        if (hasVehicles || await _context.Vehicles.AnyAsync(token))
        {
            // Check if tracks exist for the specified period
            var startDate = DateTimeOffset.Parse("2025-10-01");
            var endDate = DateTimeOffset.Parse("2025-10-10");
            bool hasTracksInPeriod = await _context.VehicleGeoTimePoints
                .AnyAsync(p => p.Time >= startDate.ToUniversalTime() && p.Time <= endDate.ToUniversalTime(), token);

            if (!hasTracksInPeriod)
            {
                // Generate GPS tracks from templates
                Console.WriteLine("Generating GPS tracks from templates...");
                int exitCode3 = await CarPark.TrackGenerator.Program.Main(new string[]
                {
                    "generate-from-templates",
                    "--templates-dir",
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Demo", "TracksData"),
                    "--start-date",
                    "2025-09-01",
                    "--end-date",
                    "2025-10-02",
                    "--active-days-ratio",
                    "0,7",
                    "--min-avg-daily-distance",
                    "50",
                    "--max-avg-daily-distance",
                    "200",
                    "--batch-size",
                    "20",
                    "--connection-string",
                    _options.ConnectionString
                });

                if (exitCode3 != 0)
                {
                    throw new Exception($"TrackGenerator exited with code {exitCode3}");
                }
            }
            else
            {
                Console.WriteLine("GPS tracks already exist for the specified period. Skipping track generation.");
            }

            // Check if rides exist for the specified period
            bool hasRidesInPeriod = await _context.Rides
                .AnyAsync(r => r.StartTime >= startDate.ToUniversalTime() && r.StartTime <= endDate.ToUniversalTime(), token);

            if (!hasRidesInPeriod)
            {
                // Generate rides from tracks
                Console.WriteLine("Generating rides from GPS tracks...");
                int exitCode4 = await CarPark.TrackGenerator.Program.Main(new string[]
                {
                    "generate-rides",
                    "--start-date",
                    "2025-10-01",
                    "--end-date",
                    "2025-10-10",
                    "--active-days-ratio",
                    "0,7",
                    "--average-rides-per-day",
                    "3",
                    "--min-ride-duration-minutes",
                    "15",
                    "--max-ride-duration-minutes",
                    "120",
                    "--connection-string",
                    _options.ConnectionString,
                    "--graphhopper-key",
                    "dummy"
                });

                if (exitCode4 != 0)
                {
                    throw new Exception($"TrackGenerator generate-rides exited with code {exitCode4}");
                }
            }
            else
            {
                Console.WriteLine("Rides already exist for the specified period. Skipping ride generation.");
            }
        }

        Console.WriteLine("Demo data initialization completed successfully.");
    }
}