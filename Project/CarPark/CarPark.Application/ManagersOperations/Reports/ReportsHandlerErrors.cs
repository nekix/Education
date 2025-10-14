namespace CarPark.ManagersOperations.Reports;

public static class ReportsHandlerErrors
{
    public const string Prefix = "ReportsHandlerErrors:";

    public const string VehicleNotFound = Prefix + "VehicleNotFound";
    public const string ManagerNotAllowedToVehicle = Prefix + "ManagerNotAllowedToVehicle";
    public const string UnknownPeriodType = Prefix + "UnknownPeriodType";
    public const string EnterpriseNotFound = Prefix + "EnterpriseNotFound";
    public const string ManagerNotAllowedToEnterprise = Prefix + "ManagerNotAllowedToEnterprise";
}