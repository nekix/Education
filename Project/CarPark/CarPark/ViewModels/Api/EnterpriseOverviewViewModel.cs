using System.Text.Json.Serialization;

namespace CarPark.ViewModels.Api;

public class EnterpriseOverviewViewModel
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required string LegalAddress { get; set; }

    //public required List<VehicleViewModel> Vehicles { get; set; }

    public required List<DriverViewModel> Drivers { get; set; }

    public class VehicleViewModel
    {
        public required int Id { get; set; }

        public required int ModelId { get; set; }

        public required string VinNumber { get; set; }

        public required decimal Price { get; set; }

        public required int ManufactureYear { get; set; }

        public required int Mileage { get; set; }

        public required string Color { get; set; }

        public required List<DriverAssignmentViewModel> DriverAssignments { get; set; }

        public class DriverAssignmentViewModel
        {
            public  required int DriverId { get; set; }

            public required bool IsActive { get; set; }
        }
    }

    public class DriverViewModel
    {
        public required int Id { get; set; }

        //public required string FullName { get; set; }

        //public required string DriverLicenseNumber { get; set; }

        public required List<VehicleAssignmentViewModel> VehiclesAssignments { get; set; }

        public class VehicleAssignmentViewModel
        {
            public required int VehicleId { get; set; }

            public required bool IsActive { get; set; }
        }
    }
}