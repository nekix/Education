using System.ComponentModel;

namespace CarPark.ViewModels.Enterprises;

public class EnterpriseViewModel
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    [DisplayName("Legal Address")]
    public required string LegalAddress { get; set; }
}