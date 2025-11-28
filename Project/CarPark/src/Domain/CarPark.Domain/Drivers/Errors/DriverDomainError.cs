using FluentResults;

namespace CarPark.Drivers.Errors;

public sealed class DriverDomainError : Error
{
    public string Code { get; private init; }

    public DriverDomainError(string code)
    {
        Code = code;
    }
}