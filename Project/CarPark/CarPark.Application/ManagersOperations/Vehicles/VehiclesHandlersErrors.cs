using CarPark.Errors;

namespace CarPark.ManagersOperations.Vehicles;

public static class VehiclesHandlersErrors
{
    public static readonly WebApiError VehicleNotExist = new WebApiError(404, "Vehicle not found.");
    public static readonly WebApiError ManagerNotAllowedToEnterprise = new WebApiError(403, "Manager is not allowed to access this enterprise.");
    public static readonly WebApiError ManagerNotAllowedToVehicle = ManagerNotAllowedToEnterprise;
    public static readonly WebApiError ModelNotExist = new WebApiError(404, "Model not found.");
    public static readonly WebApiError AssignedDriversNotExist = new WebApiError(400, "One or more assigned drivers do not exist.");
    public static readonly WebApiError ActiveAssignedDriversNotExist = new WebApiError(400, "Active assigned driver does not exist.");
    public static readonly WebApiError ForbidDeleteVehicleWithAssignedDrivers = new WebApiError(409, "Cannot delete vehicle with assigned drivers.");
}