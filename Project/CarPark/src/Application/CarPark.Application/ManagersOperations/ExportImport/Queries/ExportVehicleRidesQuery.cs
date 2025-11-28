using CarPark.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.ExportImport.Queries;

public class ExportVehicleRidesQuery : BaseManagerCommandQuery, IQuery<Result<List<VehicleRideExportImportDto>>>
{
    public required Guid VehicleId { get; set; }

    public required DateTimeOffset RidesStartTime { get; set; }

    public required DateTimeOffset RidesEndTime { get; set; }
}