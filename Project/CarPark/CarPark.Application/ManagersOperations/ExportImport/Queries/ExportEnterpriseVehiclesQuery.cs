using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.ExportImport.Queries;

public class ExportEnterpriseVehiclesQuery : BaseManagerCommandQuery, IQuery<Result<List<VehicleExportImportDto>>>
{
    public required Guid EnterpriseId { get; set; }
}