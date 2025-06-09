namespace CarPark.Models;

public class Model
{
    public int Id { get; set; }

    public string ModelName { get; set; }

    public string VehicleType { get; set; }

    public int SeatsCount { get; set; }

    public double MaxLoadingWeightKg { get; set; }

    public double EnginePowerKW { get; set; }

    public string TransmissionType { get; set; }

    public string FuelSystemType { get; set; }

    public string FuelTankVolumeLiters { get; set; }

    public static Model NoName =>
        new Model
        {
            Id = -1,
            ModelName = "NoName",
            VehicleType = string.Empty,
            SeatsCount = default,
            MaxLoadingWeightKg = default,
            EnginePowerKW = default,
            TransmissionType = string.Empty,
            FuelSystemType = string.Empty,
            FuelTankVolumeLiters = string.Empty
        };
}