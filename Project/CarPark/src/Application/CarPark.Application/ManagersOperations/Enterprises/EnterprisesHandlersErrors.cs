using CarPark.Errors;

namespace CarPark.ManagersOperations.Enterprises;

public static class EnterprisesHandlersErrors
{
    public static readonly WebApiError EnterpriseNotExist = new WebApiError(404, "Enterprise not found.");
    public static readonly WebApiError ManagerNotAllowedToEnterprise = new WebApiError(403, "Manager is not allowed to access this enterprise.");
    public static readonly WebApiError ForbidDeleteEnterpriseWithOtherManagers = new WebApiError(409, "Cannot delete enterprise with other managers.");
    public static readonly WebApiError ForbidDeleteWithAnyVehicles = new WebApiError(409, "Cannot delete enterprise with vehicles.");
    public static readonly WebApiError ForbidDeleteWithAnyDrivers = new WebApiError(409, "Cannot delete enterprise with drivers.");
}