namespace CarPark.ManagersOperations.Vehicles;

public static class VehiclesHandlersErrors
{
    public const string Prefix = "VehicleError:";

    public const string VehicleNotExist = Prefix + "VehicleNotExist";
    public const string ManagerNotAllowedToEnterprise = Prefix + "ManagerNotAllowedToEnterprise";
    public const string ManagerNotAllowedToVehicle = Prefix + "ManagerNotAllowedToVehicle";
    public const string ModelNotExist = Prefix + "ModelNotExist";
    public const string AssignedDriversNotExist = Prefix + "AssignedDriversNotExist";
    public const string ActiveAssignedDriversNotExist = Prefix + "ActiveAssignedDriversNotExist";
    public const string ForbidDeleteVehicleWithAssignedDrivers = Prefix + "ForbidDeleteVehicleWithAssignedDrivers";
}