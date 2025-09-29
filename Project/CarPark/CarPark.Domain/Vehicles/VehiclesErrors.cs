namespace CarPark.Vehicles;

public static class VehiclesErrors
{
    public const string Prefix = "Vehicles";

    public const string ModelMustBeDefined = Prefix + "ModelMustBeDefined";
    public const string EnterpriseMustBeDefined = Prefix + "EnterpriseMustBeDefined";

    public const string AssignedDriverFromAnotherEnterprise = Prefix + "AssignedDriverFromAnotherEnterprise";
    public const string DuplicatedAssignedDriver = Prefix + "DuplicatedAssignedDriver";
    public const string BeingRemovedAssignedDriverIsActive = Prefix + "BeingRemovedAssignedDriverIsActive";

    public const string ActiveAssignedDriverFromAnotherEnterprise = Prefix + "ActiveAssignedDriverFromAnotherEnterprise";
    public const string ActiveAssignedDriverMustBeInAssignedDrivers = Prefix + "ActiveAssignedDriverMustBeInAssignedDrivers";

    public const string ForbidChangeEnterpriseWhenExistAssignedDrivers = Prefix + "ForbidChangeEnterpriseWhenExistAssignedDrivers";

    public static IEnumerable<string> GetErrors()
    {
        yield return ModelMustBeDefined;
        yield return EnterpriseMustBeDefined;
        yield return AssignedDriverFromAnotherEnterprise;
        yield return DuplicatedAssignedDriver;
        yield return BeingRemovedAssignedDriverIsActive;
        yield return ActiveAssignedDriverFromAnotherEnterprise;
        yield return ActiveAssignedDriverMustBeInAssignedDrivers;
        yield return ForbidChangeEnterpriseWhenExistAssignedDrivers;
    }
}