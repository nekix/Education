using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CarPark.ViewModels.Vehicles
{
    public class CreateUpdateVehicleRequest
    {
        [Required]
        public required Guid ModelId { get; set; }

        [Required]
        [FromRoute(Name = "enterpriseId")]
        public required Guid EnterpriseId { get; set; }

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

        [Required]
        public required DateTimeOffset AddedToEnterpriseAt { get; set; }
    }

    public class ModelOverview
    {
        public required Guid Id { get; set; }

        public required string ModelName { get; set; }
    }

    public class EnterpriseOverview
    {
        public required Guid Id { get; set; }

        public required string EnterpriseName { get; set; }
    }
}
