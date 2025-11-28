using CarPark.Drivers;
using CarPark.Enterprises;
using CarPark.Models;
using FluentResults;
using static CarPark.Vehicles.Errors.VehicleDomainErrorCodes;
using CarPark.Vehicles.Errors;

namespace CarPark.Vehicles;

public sealed class Vehicle
{
    public Guid Id { get; private init; }

    public Model Model { get; private set; }

    public Enterprise Enterprise { get; private set; }

    public string VinNumber { get; private set; }

    public decimal Price { get; private set; }

    public int ManufactureYear { get; private set; }

    public int Mileage { get; private set; }

    public string Color { get; private set; }

    private List<Driver> _assignedDrivers;
    public IReadOnlyList<Driver> AssignedDrivers => _assignedDrivers.AsReadOnly();

    public Driver? ActiveAssignedDriver { get; private set; }

    public DateTimeOffset AddedToEnterpriseAt { get; private set; }

    #pragma warning disable CS8618
    [Obsolete("Only for ORM, fabric method and deserialization! Do not use!")]
    private Vehicle()
    {
        // Only for ORM and deserialization! Do not use!
    }
    #pragma warning restore CS8618

    internal static Result<Vehicle> Create(VehicleCreateData data)
    {
        Result validationResult = ValidateVinNumber(data.VinNumber)
            .Bind(() => ValidatePrice(data.Price))
            .Bind(() => ValidateManufactureYear(data.ManufactureYear))
            .Bind(() => ValidateMileage(data.Mileage))
            .Bind(() => ValidateColor(data.Color));

        if (validationResult.IsFailed)
            return Result.Fail<Vehicle>(validationResult.Errors);

        #pragma warning disable CS0618
        Vehicle vehicle = new Vehicle
        {
            Id = data.Id,
            VinNumber = data.VinNumber,
            Price = data.Price,
            ManufactureYear = data.ManufactureYear,
            Mileage = data.Mileage,
            Color = data.Color,
            _assignedDrivers = new List<Driver>(),
            ActiveAssignedDriver = null,
            AddedToEnterpriseAt = data.AddedToEnterpriseAt
        };
        #pragma warning restore CS0618

        return vehicle.SetModel(data.Model)
            .Bind(() => vehicle.SetEnterprise(data.Enterprise))
            .Bind(() => vehicle.SetAssignedDrivers(data.AssignedDrivers))
            .Bind(() => vehicle.SetActiveAssignedDriver(data.ActiveAssignedDriver));
    }

    internal static Result Update(Vehicle vehicle, VehicleUpdateData data)
    {
        Result validationResult = ValidateVinNumber(data.VinNumber)
            .Bind(() => ValidatePrice(data.Price))
            .Bind(() => ValidateManufactureYear(data.ManufactureYear))
            .Bind(() => ValidateMileage(data.Mileage))
            .Bind(() => ValidateColor(data.Color));

        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        vehicle.VinNumber = data.VinNumber;
        vehicle.Price = data.Price;
        vehicle.ManufactureYear = data.ManufactureYear;
        vehicle.Mileage = data.Mileage;
        vehicle.Color = data.Color;

        Result<Vehicle> result = vehicle.SetModel(data.Model);
        if (result.IsFailed) return Result.Fail(result.Errors);

        result = vehicle.SetEnterprise(data.Enterprise);
        if (result.IsFailed) return Result.Fail(result.Errors);

        result = vehicle.SetAssignedDrivers(data.AssignedDrivers);
        if (result.IsFailed) return Result.Fail(result.Errors);

        result = vehicle.SetActiveAssignedDriver(data.ActiveAssignedDriver);
        if (result.IsFailed) return Result.Fail(result.Errors);

        vehicle.AddedToEnterpriseAt = data.AddedToEnterpriseAt;

        return Result.Ok();
    }

    private static Result ValidateVinNumber(string vinNumber)
    {
        if (string.IsNullOrWhiteSpace(vinNumber))
            return Result.Fail(new VehicleDomainError(VinNumberRequired));
        return Result.Ok();
    }

    private static Result ValidatePrice(decimal price)
    {
        if (price <= 0)
            return Result.Fail(new VehicleDomainError(PriceMustBePositive));
        return Result.Ok();
    }

    private static Result ValidateManufactureYear(int manufactureYear)
    {
        if (manufactureYear <= 0)
            return Result.Fail(new VehicleDomainError(ManufactureYearMustBePositive));
        return Result.Ok();
    }

    private static Result ValidateMileage(int mileage)
    {
        if (mileage < 0)
            return Result.Fail(new VehicleDomainError(MileageMustBeNonNegative));
        return Result.Ok();
    }
    
    private static Result ValidateColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return Result.Fail(new VehicleDomainError(ColorRequired));
        return Result.Ok();
    }

    private Result SetAssignedDrivers(IEnumerable<Driver> drivers)
    {
        List<Driver> newDrivers = drivers.ToList();

        // Check for duplicates
        bool hasDuplicates = newDrivers.GroupBy(d => d.Id)
            .Where(g => g.Count() > 1)
            .Any();

        if (hasDuplicates)
            return Result.Fail(new VehicleDomainError(DuplicatedAssignedDriver));

        // Check enterprises
        if (newDrivers.Any(d => d.EnterpriseId != Enterprise.Id))
            return Result.Fail(new VehicleDomainError(AssignedDriverFromAnotherEnterprise));

        // Check what is being removed
        List<Driver> toRemove = _assignedDrivers
            .Where(ad => !newDrivers.Any(nd => nd.Id == ad.Id))
            .ToList();

        // Cannot remove the active driver
        if (toRemove.Any(rd => rd.Id == ActiveAssignedDriver?.Id))
            return Result.Fail(new VehicleDomainError(BeingRemovedAssignedDriverIsActive));

        // For new drivers
        List<Driver> toAdd = newDrivers
            .Where(nd => !_assignedDrivers.Any(cd => cd.Id == nd.Id))
            .ToList();

        // Apply changes
        foreach (Driver d in toRemove)
        {
            _assignedDrivers.Remove(d);
        }

        foreach (Driver d in toAdd)
        {
            _assignedDrivers.Add(d);
        }

        return Result.Ok();
    }

    private Result SetModel(Model model)
    {
        if (model == null)
        {
            return Result.Fail(new VehicleDomainError(ModelMustBeDefined));
        }

        Model = model;

        return Result.Ok();
    }

    private Result SetEnterprise(Enterprise enterprise)
    {
        if (enterprise == null)
        {
            return Result.Fail(new VehicleDomainError(EnterpriseMustBeDefined));
        }

        if (AssignedDrivers.Count > 0)
        {
            return Result.Fail(new VehicleDomainError(ForbidChangeEnterpriseWhenExistAssignedDrivers));
        }

        Enterprise = enterprise;

        return Result.Ok();
    }

    private Result SetActiveAssignedDriver(Driver? newActiveAssignedDriver)
    {
        if (newActiveAssignedDriver == null)
        {
            ActiveAssignedDriver = newActiveAssignedDriver;
            return Result.Ok();
        }

        if (newActiveAssignedDriver.EnterpriseId != Enterprise.Id)
        {
            return Result.Fail(new VehicleDomainError(ActiveAssignedDriverFromAnotherEnterprise));
        }

        if (newActiveAssignedDriver != null && AssignedDrivers.All(d => d.Id != newActiveAssignedDriver.Id))
        {
            return Result.Fail(new VehicleDomainError(ActiveAssignedDriverMustBeInAssignedDrivers));
        }

        ActiveAssignedDriver = newActiveAssignedDriver;

        return Result.Ok();
    }
}