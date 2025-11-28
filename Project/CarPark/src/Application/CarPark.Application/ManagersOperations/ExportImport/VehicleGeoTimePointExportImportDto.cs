namespace CarPark.ManagersOperations.ExportImport;

public class VehicleGeoTimePointExportImportDto
{
    public Guid Id { get; set; }

    public Guid VehicleId { get; set; }

    public required double X { get; set; }

    public required double Y { get; set; }

    public required DateTimeOffset Time { get; set; }
}