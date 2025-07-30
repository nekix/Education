using CarPark.Models.Managers;

namespace CarPark.Models.Enterprises;

public sealed class Enterprise
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required string LegalAddress { get; set; }
    
    public required List<Manager> Managers { get; set; }
}