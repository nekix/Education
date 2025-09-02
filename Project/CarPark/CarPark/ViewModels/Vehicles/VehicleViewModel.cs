using System.ComponentModel.DataAnnotations;

namespace CarPark.ViewModels.Vehicles;

public class VehicleViewModel
{
    public required int Id { get; set; }

    public required ModelViewModel Model { get; set; }

    public required EnterpriseViewModel Enterprise { get; set; }

    public required string VinNumber { get; set; }

    public required decimal Price { get; set; }

    public required int ManufactureYear { get; set; }

    public required int Mileage { get; set; }

    public required string Color { get; set; }

    public class ModelViewModel
    {
        public required int Id { get; set; }

        [Display(Name = "Model name")]
        public required string Name { get; set; }
    }

    public class EnterpriseViewModel
    {
        public required int Id { get; set; }

        [Display(Name = "Enterprise name")]
        public required string Name { get; set; }
    }
}