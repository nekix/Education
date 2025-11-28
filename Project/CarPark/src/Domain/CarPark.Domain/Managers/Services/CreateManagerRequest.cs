using CarPark.Enterprises;

namespace CarPark.Managers.Services;

public record CreateManagerRequest
{
    public required Guid Id { get; init; }

    public required string IdentityUserId { get; init; }

    public required IReadOnlyList<Enterprise> Enterprises { get; init; }
}