using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.ExportImport.Commands;

public class ImportCommand : BaseManagerCommandQuery, ICommand<Result>
{
    public required List<EnterpriseExportImportDto>? Enterprises { get; set; }

    public required List<ModelExportImportDto>? Models { get; set; }

    public required List<VehicleExportImportDto>? Vehicles { get; set; }

    public required List<VehicleRideExportImportDto>? Rides { get; set; }

    public required List<VehicleGeoTimePointExportImportDto>? Tracks { get; set; }
}