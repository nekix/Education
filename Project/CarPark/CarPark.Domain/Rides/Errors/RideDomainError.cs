using FluentResults;

namespace CarPark.Rides.Errors;

public sealed class RideDomainError : Error
{
    public string Code { get; private init; }

    public RideDomainError(string code)
    {
        Code = code;
    }
}