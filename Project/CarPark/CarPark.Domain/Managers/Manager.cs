using CarPark.Enterprises;

namespace CarPark.Managers;

public class Manager
{
    public required int Id { get; set; }

    public required string IdentityUserId { get; set; }

    public required List<Enterprise> Enterprises { get; set; }
}