namespace CarPark.ManagersOperations.ExportImport.Queries;

public class VehicleTracksExportDto
{
    public required List<VehicleGeoTimePointExportImportDto> Tracks { get; set; }
}