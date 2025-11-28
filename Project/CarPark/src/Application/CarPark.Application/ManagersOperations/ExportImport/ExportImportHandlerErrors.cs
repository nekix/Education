using CarPark.Errors;

namespace CarPark.ManagersOperations.ExportImport;

public static class ExportImportHandlerErrors
{
    public static readonly WebApiError ManagerNotAllowedToEnterprise = new WebApiError(403, "Manager not allowed to enterprise.");
    public static readonly WebApiError VehicleNotExist = new WebApiError(404, "Vehicle not found.");
    public static readonly WebApiError TimeZoneNotExist = new WebApiError(400, "Time zone not found.");
    public static readonly WebApiError EnterpriseNotFound = new WebApiError(404, "Enterprise not found.");
    public static readonly WebApiError ModelNotFound = new WebApiError(404, "Model not found.");
    public static readonly WebApiError RidePointNotFound = new WebApiError(404, "Ride point not found.");
}