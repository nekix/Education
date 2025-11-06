namespace CarPark.ManagersOperations.Tracks;

public static class TrackHandlersErrors
{
    private const string Prefix = "TrackError";

    public const string VehicleNotFound = Prefix + "VehicleNotFound";
    public const string ManagerNotAllowedToVehicle = Prefix + "ManagerNotAllowedToVehicle";

    public const string GpxContainLessThanTwoPoints = Prefix + "GpxContainLessThanTwoPoints";
    public const string GpxUnknowsGeometryType = Prefix + " GpxUnknowsGeometryType";
    public const string GpxPointsDateTimeNotDefined = Prefix + "GpxPointsDateTimeNotDefined";

    public const string TracksOverlapWithExisting = Prefix + "TracksOverlapWithExisting";
    public const string TracksNotSequential = Prefix + "TracksNotSequential";
    public const string GpxMustContainOneTrack = Prefix + "GpxMustContainOneTrack";
}