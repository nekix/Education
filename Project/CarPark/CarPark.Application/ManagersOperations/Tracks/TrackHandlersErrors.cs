namespace CarPark.ManagersOperations.Tracks;

public static class TrackHandlersErrors
{
    private const string Prefix = "TrackError";

    public const string VehicleNotFound = Prefix + "VehicleNotFound";
    public const string ManagerNotAllowedToVehicle = Prefix + "ManagerNotAllowedToVehicle";
}