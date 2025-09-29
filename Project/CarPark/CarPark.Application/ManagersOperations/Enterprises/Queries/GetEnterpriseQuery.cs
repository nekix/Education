using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Enterprises.Queries;

public class GetEnterpriseQuery : BaseManagerCommandQuery, IQuery<Result<EnterpriseDto>>
{
    public required int EnterpriseId { get; set; }
}