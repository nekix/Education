using CarPark.Reports;
using CarPark.Reports.Abstract;
using CarPark.Shared.CQ;
using CarPark.Shared.DateTimes;
using FluentResults;

namespace CarPark.ManagersOperations.Reports.Queries;

public class GetVehicleMileageReportQuery : BaseManagerCommandQuery, IQuery<Result<VehicleMileagePeriodReport>>
{
    public required Guid VehicleId { get; init; }

    public required PeriodType Period { get; init; }

    public required UtcDateTimeOffset StartDate { get; init; }

    public required UtcDateTimeOffset EndDate { get; init; }
}