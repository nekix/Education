using CarPark.CQ;
using CarPark.Reports;
using FluentResults;

namespace CarPark.ManagersOperations.Reports.Queries;

public class GetEnterpriseModelsReportQuery : BaseManagerCommandQuery, IQuery<Result<EnterpriseVehiclesModelsReport>>
{
    public required Guid EnterpriseId { get; init; }
}