using CarPark.Enterprises;

namespace CarPark.Managers;

internal sealed record ManagerCreateData
{
    public required Guid Id { get; init; }

    public required string IdentityUserId { get; init; }

    public required IReadOnlyList<Enterprise> Enterprises { get; init; }
}