using CarPark.Errors;

namespace CarPark.ManagersOperations.Rides;

public static class RidesHandlerErrors
{
    public static readonly WebApiError VehicleNotFound = new WebApiError(404, "Vehicle not found.");
    public static readonly WebApiError ManagerNotAllowedToVehicle = new WebApiError(403, "Manager is not allowed to access this vehicle.");
    public static readonly WebApiError RidesOverlapWithExisting = new WebApiError(409, "Rides overlap with existing rides.");
}