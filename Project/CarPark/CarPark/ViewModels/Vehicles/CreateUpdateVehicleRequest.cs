using System.ComponentModel.DataAnnotations;

namespace CarPark.ViewModels.Vehicles
{
    public class CreateUpdateVehicleRequest
    {
        [Required]
        public required int ModelId { get; set; }

        [Required]
        public required int EnterpriseId { get; set; }

        [Required]
        public required string VinNumber { get; set; }

        [Required]
        public required decimal Price { get; set; }

        [Required]
        public required int ManufactureYear { get; set; }

        [Required]
        public required int Mileage { get; set; }

        [Required]
        public required string Color { get; set; }
    }

    public class ModelOverview
    {
        public required int Id { get; set; }

        public required string ModelName { get; set; }
    }

    public class EnterpriseOverview
    {
        public required int Id { get; set; }

        public required string EnterpriseName { get; set; }
    }
}
