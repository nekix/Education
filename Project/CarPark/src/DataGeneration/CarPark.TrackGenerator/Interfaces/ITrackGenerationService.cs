using CarPark.TrackGenerator.Models;
using NetTopologySuite.Geometries;

namespace CarPark.TrackGenerator.Interfaces;

public interface ITrackGenerationService
{
    Task<Track> GenerateTrackAsync(TrackGenerationOptions options);
    Task<Track> GenerateTrackFromPointAsync(Point startPoint, TrackGenerationOptions options);
}
