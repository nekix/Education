using CarPark.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Enterprises.Queries;

public class GetEnterpriseQuery : BaseManagerCommandQuery, IQuery<Result<EnterpriseDto>>
{
    public required Guid EnterpriseId { get; set; }
}