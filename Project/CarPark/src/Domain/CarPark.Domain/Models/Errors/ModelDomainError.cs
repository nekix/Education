using FluentResults;

namespace CarPark.Models.Errors;

public sealed class ModelDomainError : Error
{
    public string Code { get; private init; }

    public ModelDomainError(string code)
    {
        Code = code;
    }
}