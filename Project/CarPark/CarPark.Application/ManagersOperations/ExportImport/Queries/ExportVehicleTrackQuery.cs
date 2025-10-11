using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.ExportImport.Queries;

public class ExportVehicleTrackQuery : BaseManagerCommandQuery, IQuery<Result<List<VehicleGeoTimePointExportImportDto>>>
{
    public required Guid VehicleId { get; set; }

    public required DateTimeOffset TrackStartTime { get; set; }

    public required DateTimeOffset TrackEndTime { get; set; }
}