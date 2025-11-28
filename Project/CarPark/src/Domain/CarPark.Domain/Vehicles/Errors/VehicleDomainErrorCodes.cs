namespace CarPark.Vehicles.Errors;

public static class VehicleDomainErrorCodes
{
    public const string VinNumberRequired = "VinNumberRequired";
    public const string PriceMustBePositive = "PriceMustBePositive";
    public const string ManufactureYearMustBePositive = "ManufactureYearMustBePositive";
    public const string MileageMustBeNonNegative = "MileageMustBeNonNegative";
    public const string ColorRequired = "ColorRequired";
    public const string ModelMustBeDefined = "ModelMustBeDefined";
    public const string EnterpriseMustBeDefined = "EnterpriseMustBeDefined";
    public const string AssignedDriverFromAnotherEnterprise = "AssignedDriverFromAnotherEnterprise";
    public const string DuplicatedAssignedDriver = "DuplicatedAssignedDriver";
    public const string BeingRemovedAssignedDriverIsActive = "BeingRemovedAssignedDriverIsActive";
    public const string ActiveAssignedDriverFromAnotherEnterprise = "ActiveAssignedDriverFromAnotherEnterprise";
    public const string ActiveAssignedDriverMustBeInAssignedDrivers = "ActiveAssignedDriverMustBeInAssignedDrivers";
    public const string ForbidChangeEnterpriseWhenExistAssignedDrivers = "ForbidChangeEnterpriseWhenExistAssignedDrivers";
    public const string CannotDeleteVehicleWithAssignedDrivers = "CannotDeleteVehicleWithAssignedDrivers";
}