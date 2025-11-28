namespace CarPark.Models.Services;

public record UpdateModelRequest
{
    public required string ModelName { get; init; }

    public required string VehicleType { get; init; }

    public required int SeatsCount { get; init; }

    public required double MaxLoadingWeightKg { get; init; }

    public required double EnginePowerKW { get; init; }

    public required string TransmissionType { get; init; }

    public required string FuelSystemType { get; init; }

    public required string FuelTankVolumeLiters { get; init; }
}