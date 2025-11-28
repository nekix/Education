using FluentResults;

namespace CarPark.Vehicles.Errors;

public class VehicleDomainError : Error
{
    public string Code { get; }

    public VehicleDomainError(string code) : base(code)
    {
        Code = code;
    }
}