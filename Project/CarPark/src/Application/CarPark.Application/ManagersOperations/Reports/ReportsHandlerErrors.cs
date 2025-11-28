using CarPark.Errors;

namespace CarPark.ManagersOperations.Reports;

public static class ReportsHandlerErrors
{
    public static readonly WebApiError VehicleNotFound = new WebApiError(404, "Vehicle not found.");
    public static readonly WebApiError ManagerNotAllowedToVehicle = new WebApiError(403, "Manager is not allowed to access this vehicle.");
    public static readonly WebApiError UnknownPeriodType = new WebApiError(400, "Unknown period type.");
    public static readonly WebApiError EnterpriseNotFound = new WebApiError(404, "Enterprise not found.");
    public static readonly WebApiError ManagerNotAllowedToEnterprise = new WebApiError(403, "Manager is not allowed to access this enterprise.");
}