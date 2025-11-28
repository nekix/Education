using CarPark.Errors;

namespace CarPark.ManagersOperations.Tracks;

public static class TrackHandlersErrors
{
    public static readonly WebApiError VehicleNotFound = new WebApiError(404, "Vehicle not found.");
    public static readonly WebApiError ManagerNotAllowedToVehicle = new WebApiError(403, "Manager is not allowed to access this vehicle.");

    public static readonly WebApiError GpxContainLessThanTwoPoints = new WebApiError(400, "GPX file contains less than two points.");
    public static readonly WebApiError GpxUnknowsGeometryType = new WebApiError(400, "Unknown geometry type in GPX file.");
    public static readonly WebApiError GpxPointsDateTimeNotDefined = new WebApiError(400, "GPX points must have date and time defined.");

    public static readonly WebApiError TracksOverlapWithExisting = new WebApiError(409, "Tracks overlap with existing tracks.");
    public static readonly WebApiError TracksNotSequential = new WebApiError(400, "Tracks are not in sequential time order.");
    public static readonly WebApiError GpxMustContainOneTrack = new WebApiError(400, "GPX file must contain exactly one track.");
}