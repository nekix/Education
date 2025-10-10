using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.ExportImport.Queries;

public class ExportEnterpriseVehiclesQuery : BaseManagerCommandQuery, IQuery<Result<List<VehicleExportImportDto>>>
{
    public required int EnterpriseId { get; set; }
}