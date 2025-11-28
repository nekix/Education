using CarPark.Errors;

namespace CarPark.ManagersOperations.Drivers;

public static class DriversHandlerErrors
{
    public static readonly WebApiError DriverNotExist = new WebApiError(404, "Driver not found.");
    public static readonly WebApiError ManagerNotAllowedToVehicle = new WebApiError(403, "Manager is not allowed to access this vehicle.");
}