namespace CarPark.Rides.Errors;

public static class RideDomainErrorCodes
{
    public const string StartTimeMustBeLessThanOrEqualToEndTime = "StartTimeMustBeLessThanOrEqualToEndTime";
    public const string StartTimeMustBeGreaterThanOrEqualToStartPointTime = "StartTimeMustBeGreaterThanOrEqualToStartPointTime";
    public const string EndTimeMustBeGreaterThanOrEqualToEndPointTime = "EndTimeMustBeGreaterThanOrEqualToEndPointTime";
    public const string StartPointVehicleMustMatchRideVehicle = "StartPointVehicleMustMatchRideVehicle";
    public const string EndPointVehicleMustMatchRideVehicle = "EndPointVehicleMustMatchRideVehicle";
}