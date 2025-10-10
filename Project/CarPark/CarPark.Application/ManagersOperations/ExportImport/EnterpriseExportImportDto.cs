namespace CarPark.ManagersOperations.ExportImport;

public class EnterpriseExportImportDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required string LegalAddress { get; set; }

    public required string? IanaTzId { get; set; }

    public required string? WindowsTzId { get; set; }
}