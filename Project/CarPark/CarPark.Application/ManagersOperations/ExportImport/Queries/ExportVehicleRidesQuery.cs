using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.ExportImport.Queries;

public class ExportVehicleRidesQuery : BaseManagerCommandQuery, IQuery<Result<List<VehicleRideExportImportDto>>>
{
    public required int VehicleId { get; set; }

    public required DateTimeOffset RidesStartTime { get; set; }

    public required DateTimeOffset RidesEndTime { get; set; }
}