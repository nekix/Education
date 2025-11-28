using CarPark.Errors;

namespace CarPark.ManagersOperations;

public static class ManagersOperationsErrors
{
    public static readonly WebApiError ManagerNotExist = new WebApiError(403, "Manager does not exist.");
}