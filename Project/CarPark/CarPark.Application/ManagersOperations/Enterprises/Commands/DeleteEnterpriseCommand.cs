using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Enterprises.Commands;

public class DeleteEnterpriseCommand : BaseManagerCommandQuery, ICommand<Result>
{
    public required Guid EnterpriseId { get; set; }
}