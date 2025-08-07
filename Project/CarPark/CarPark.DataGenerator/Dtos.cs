namespace CarPark.DataGenerator
{
    public class SerializableData
    {
        public required List<VehicleDto> Vehicles { get; set; }
        public required List<DriverDto> Drivers { get; set; }
    }

    public class EnterpriseDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string LegalAddress { get; set; }
    }

    public class ModelDto
    {
        public required int Id { get; set; }
        public required string ModelName { get; set; }
        public required string VehicleType { get; set; }
        public required int SeatsCount { get; set; }
        public required double MaxLoadingWeightKg { get; set; }
        public required double EnginePowerKW { get; set; }
        public required string TransmissionType { get; set; }
        public required string FuelSystemType { get; set; }
        public required string FuelTankVolumeLiters { get; set; }
    }

    public class VehicleDto
    {
        public required int Id { get; set; }
        public required int ModelId { get; set; }
        public required int EnterpriseId { get; set; }
        public required string VinNumber { get; set; }
        public required decimal Price { get; set; }
        public required int ManufactureYear { get; set; }
        public required int Mileage { get; set; }
        public required string Color { get; set; }
        public required int? ActiveDriverId { get; set; }
        public required List<int> AssignedDriverIds { get; set; }
    }

    public class DriverDto
    {
        public required int Id { get; set; }
        public required int EnterpriseId { get; set; }
        public required string FullName { get; set; }
        public required string DriverLicenseNumber { get; set; }
        public required int? ActiveVehicleId { get; set; }
        public required List<int> AssignedVehicleIds { get; set; }
    }
}
