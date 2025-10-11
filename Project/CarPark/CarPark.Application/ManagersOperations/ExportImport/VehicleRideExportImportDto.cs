namespace CarPark.ManagersOperations.ExportImport;

public class VehicleRideExportImportDto
{
    public required Guid Id { get; set; }

    public required Guid VehicleId { get; set; }

    public required DateTimeOffset StartTime { get; set; }

    public required DateTimeOffset EndTime { get; set; }

    public Guid StartVehicleGeoTimePointId { get; set; }

    public Guid EndVehicleGeoTimePointId { get; set; }
}