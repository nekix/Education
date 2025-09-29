namespace CarPark.Models;

public sealed class Model
{
    public required int Id { get; set; }

    public required string ModelName { get; set; }

    public required string VehicleType { get; set; }

    public required int SeatsCount { get; set; }

    public required double MaxLoadingWeightKg { get; set; }

    public required double EnginePowerKW { get; set; }

    public required string TransmissionType { get; set; }

    public required string FuelSystemType { get; set; }

    public required string FuelTankVolumeLiters { get; set; }
}