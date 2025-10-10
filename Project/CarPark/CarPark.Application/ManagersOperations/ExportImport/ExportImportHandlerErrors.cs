namespace CarPark.ManagersOperations.ExportImport;

public static class ExportImportHandlerErrors
{
    private const string Prefix = "ExportImport:";

    public const string ManagerNotAllowedToEnterprise = Prefix + "ManagerNotAllowedToEnterprise";
    public const string VehicleNotExist = Prefix + "VehicleNotExist";
    public const string TimeZoneNotExist = Prefix + "TimeZoneNotExist";
    public const string EnterpriseNotFound = Prefix + "EnterpriseNotFound";
    public const string ModelNotFound = Prefix + "ModelNotFound";
    public const string RidePointNotFound = Prefix + "RidePointNotFound";
}