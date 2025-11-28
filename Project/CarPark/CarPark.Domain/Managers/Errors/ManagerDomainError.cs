using FluentResults;

namespace CarPark.Managers.Errors;

public sealed class ManagerDomainError : Error
{
    public string Code { get; private init; }

    public ManagerDomainError(string code)
    {
        Code = code;
    }
}