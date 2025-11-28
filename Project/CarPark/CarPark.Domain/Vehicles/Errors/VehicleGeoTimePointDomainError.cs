using FluentResults;

namespace CarPark.Vehicles.Errors;

public class VehicleGeoTimePointDomainError : Error
{
    public string Code { get; }

    public VehicleGeoTimePointDomainError(string code) : base(code)
    {
        Code = code;
    }
}