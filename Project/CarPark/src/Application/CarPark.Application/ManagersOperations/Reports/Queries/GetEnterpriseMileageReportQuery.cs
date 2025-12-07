using CarPark.CQ;
using CarPark.DateTimes;
using CarPark.Reports;
using CarPark.Reports.Abstract;
using FluentResults;

namespace CarPark.ManagersOperations.Reports.Queries;

public class GetEnterpriseMileageReportQuery : BaseManagerCommandQuery, IQuery<Result<EnterpriseMileagePeriodReport>>
{
    public required Guid EnterpriseId { get; init; }

    public required PeriodType Period { get; init; }

    public required UtcDateTimeOffset StartDate { get; init; }

    public required UtcDateTimeOffset EndDate { get; init; }
}