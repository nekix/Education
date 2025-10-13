using CarPark.Reports;
using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Reports.Queries;

public class GetEnterpriseModelsReportQuery : BaseManagerCommandQuery, IQuery<Result<EnterpriseVehiclesModelsReport>>
{
    public required Guid EnterpriseId { get; init; }
}