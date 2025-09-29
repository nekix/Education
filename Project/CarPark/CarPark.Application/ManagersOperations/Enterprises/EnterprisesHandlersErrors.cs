namespace CarPark.ManagersOperations.Enterprises;

public static class EnterprisesHandlersErrors
{
    private const string Prefix = "EnterprisesHandlersErrors";

    public const string EnterpriseNotExist = Prefix + "EnterpriseNotExist";
    public const string ManagerNotAllowedToEnterprise = Prefix + "ManagerNotAllowedToEnterprise";
    public const string ForbidDeleteEnterpriseWithOtherManagers = Prefix + "ForbidDeleteEnterpriseWithOtherManagers";
    public const string ForbidDeleteWithAnyVehicles = Prefix + "ForbidDeleteWithAnyVehicles";
    public const string ForbidDeleteWithAnyDrivers = Prefix + "ForbidDeleteWithAnyDrivers";
}