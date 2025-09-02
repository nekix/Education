namespace CarPark.ViewModels.Enterprises;

public class EnterpriseDetailsViewModel
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required string LegalAddress { get; set; }

    public required List<VehicleViewModel> Vehicles { get; set; }

    public class VehicleViewModel
    {
        public required int Id { get; set; }

        public required string VinNumber { get; set; }
    }
}