using CarPark.Drivers;
using CarPark.Enterprises;
using CarPark.Models;
using FluentResults;

namespace CarPark.Vehicles;

public sealed class Vehicle
{
    public Guid Id { get; private init; }

    public Model Model { get; private set; }

    public Enterprise Enterprise { get; private set; }

    public string VinNumber { get; set; }

    public decimal Price { get; set; }

    public int ManufactureYear { get; set; }

    public int Mileage { get; set; }

    public string Color { get; set; }

    private List<Driver> _assignedDrivers;
    public IReadOnlyList<Driver> AssignedDrivers => _assignedDrivers.AsReadOnly();

    public Driver? ActiveAssignedDriver { get; private set; }

    public DateTimeOffset AddedToEnterpriseAt { get; set; }

    #pragma warning disable CS8618
    [Obsolete("Only for ORM, fabric method and deserialization! Do not use!")]
    private Vehicle()
    {
        // Only for ORM and deserialization! Do not use!
    }
    #pragma warning restore CS8618

    public static Result<Vehicle> Create(
        Guid id,
        Model model,
        Enterprise enterprise,
        string vinNumber,
        decimal price,
        int manufactureYear,
        int mileage,
        string color,
        List<Driver> assignedDrivers,
        Driver? activeAssignedDriver,
        DateTimeOffset addedToEnterpriseAt)
    {
        #pragma warning disable CS0618
        Vehicle vehicle = new Vehicle
        {
            Id = id,
            VinNumber = vinNumber,
            Price = price,
            ManufactureYear = manufactureYear,
            Mileage = mileage,
            Color = color,
            _assignedDrivers = new List<Driver>(),
            ActiveAssignedDriver = activeAssignedDriver,
            AddedToEnterpriseAt = addedToEnterpriseAt
        };
        #pragma warning restore CS0618 // Type or member is obsolete

        return vehicle.SetModel(model)
            .Bind(v => v.SetEnterprise(enterprise))
            .Bind(v =>
            {
                foreach (Driver newAssignedDriver in assignedDrivers)
                {
                    Result<Vehicle> addResult = v.AddAssignedDriver(newAssignedDriver);

                    if (addResult.IsFailed)
                        return addResult;
                }

                return Result.Ok<Vehicle>(v);
            })
            .Bind(v => v.SetActiveAssignedDriver(activeAssignedDriver));
    }

    public Result<Vehicle> SetModel(Model model)
    {
        if (model == null)
        {
            return Result.Fail<Vehicle>(VehiclesErrors.ModelMustBeDefined);
        }

        Model = model;

        return Result.Ok<Vehicle>(this);
    }

    public Result<Vehicle> SetEnterprise(Enterprise enterprise)
    {
        if (enterprise == null)
        {
            return Result.Fail<Vehicle>(VehiclesErrors.EnterpriseMustBeDefined);
        }

        if (AssignedDrivers.Count > 0)
        {
            return Result.Fail<Vehicle>(VehiclesErrors.ForbidChangeEnterpriseWhenExistAssignedDrivers);
        }

        Enterprise = enterprise;

        return Result.Ok<Vehicle>(this);
    }

    public Result<Vehicle> AddAssignedDriver(Driver newAssignedDriver)
    {
        if (newAssignedDriver.EnterpriseId != Enterprise.Id)
        {
            return Result.Fail<Vehicle>(VehiclesErrors.AssignedDriverFromAnotherEnterprise);
        }

        if (AssignedDrivers.Any(d => d.Id == newAssignedDriver.Id))
        {
            return Result.Fail<Vehicle>(VehiclesErrors.DuplicatedAssignedDriver);
        }

        _assignedDrivers.Add(newAssignedDriver);

        return Result.Ok<Vehicle>(this);
    }

    public Result<Vehicle> DeleteAssignedDriver(Driver assignedDriver)
    {
        if (ActiveAssignedDriver != null && ActiveAssignedDriver.Id == assignedDriver.Id)
        {
            return Result.Fail<Vehicle>(VehiclesErrors.BeingRemovedAssignedDriverIsActive);
        }

        _assignedDrivers.Remove(assignedDriver);

        return Result.Ok<Vehicle>(this);
    }

    public Result<Vehicle> SetActiveAssignedDriver(Driver? newActiveAssignedDriver)
    {
        if (newActiveAssignedDriver == null)
        {
            ActiveAssignedDriver = newActiveAssignedDriver;
            return Result.Ok<Vehicle>(this);
        }

        if (newActiveAssignedDriver.EnterpriseId != Enterprise.Id)
        {
            return Result.Fail<Vehicle>(VehiclesErrors.ActiveAssignedDriverFromAnotherEnterprise);
        }

        if (newActiveAssignedDriver != null && AssignedDrivers.All(d => d.Id != newActiveAssignedDriver.Id))
        {
            return Result.Fail<Vehicle>(VehiclesErrors.ActiveAssignedDriverMustBeInAssignedDrivers);
        }

        ActiveAssignedDriver = newActiveAssignedDriver;

        return Result.Ok<Vehicle>(this);
    }
}