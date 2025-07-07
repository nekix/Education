namespace CarPark.Models;

public sealed class Enterprise
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string LegalAddress { get; set; }
    
    public List<Manager> Managers { get; set; }
}