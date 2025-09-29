using CarPark.TrackGenerator.Models;

namespace CarPark.TrackGenerator.Interfaces;

public interface ITrackGenerationService
{
    Task<Track> GenerateTrackAsync(TrackGenerationOptions options);
}
