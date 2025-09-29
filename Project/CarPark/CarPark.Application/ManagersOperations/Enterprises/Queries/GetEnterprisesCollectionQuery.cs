using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Enterprises.Queries;

public class GetEnterprisesCollectionQuery : BaseManagerCommandQuery, IQuery<Result<List<EnterpriseDto>>>
{

}