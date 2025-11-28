using FluentResults;

namespace CarPark.Enterprises.Errors;

public sealed class EnterpriseDomainError : Error
{
    public string Code { get; private init; }

    public EnterpriseDomainError(string code)
    {
        Code = code;
    }
}
