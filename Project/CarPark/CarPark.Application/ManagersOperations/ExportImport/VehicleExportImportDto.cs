namespace CarPark.ManagersOperations.ExportImport;

public class VehicleExportImportDto
{
    public required int Id { get; set; }

    public required int ModelId { get; set; }

    public required int EnterpriseId { get; set; }

    public required string VinNumber { get; set; }

    public required decimal Price { get; set; }

    public required int ManufactureYear { get; set; }

    public required int Mileage { get; set; }

    public required string Color { get; set; }

    public required DateTimeOffset AddedToEnterpriseAt { get; set; }
}