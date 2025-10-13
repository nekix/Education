using CarPark.Data;
using CarPark.Managers;
using CarPark.Reports;
using CarPark.Reports.Abstract;
using CarPark.Shared.CQ;
using CarPark.Shared.DateTimes;
using CarPark.Vehicles;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CarPark.ManagersOperations.Reports.Queries;

public class ReportsQueryHandler : BaseManagersHandler,
    IQueryHandler<GetVehicleMileageReportQuery, Result<VehicleMileagePeriodReport>>
    //IQueryHandler<GetEnterpriseRidesReportQuery, Result<EnterpriseRidesPeriodReport>>,
    //IQueryHandler<GetEnterpriseModelsReportQuery, Result<EnterpriseVehiclesModelsReport>>
{
    private const string GetVehicleMileagePeriodSql =
        @"
        WITH distances AS (
            SELECT
                time,
                ST_Distance(
                    location::geography,
                    LAG(location) OVER (ORDER BY time)::geography
                ) AS segment_distance,
                EXTRACT(EPOCH FROM (time - LAG(time) OVER (ORDER BY time))) / 60 AS diff_minutes
            FROM car_park.vehicle_geo_time_point
            WHERE vehicle_id = @vehicleId
              AND time BETWEEN @startTime AND @endTime
        )
        SELECT
            DATE_TRUNC(@period, time)::timestamptz AS date,
            SUM(segment_distance)/1000.0 AS mileage_in_km
        FROM distances
        WHERE diff_minutes <= 10
        GROUP BY Date
        ORDER BY Date;
        ";

    public ReportsQueryHandler(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Result<VehicleMileagePeriodReport>> Handle(GetVehicleMileageReportQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<VehicleMileagePeriodReport>();

        Manager manager = getManager.Value;

        Vehicle? vehicle = await DbContext.Vehicles
            .Include(v => v.Enterprise)
            .FirstOrDefaultAsync(v => v.Id == query.VehicleId);

        if (vehicle == null)
            return Result.Fail<VehicleMileagePeriodReport>(ReportsHandlerErrors.VehicleNotFound);

        if (manager.Enterprises.All(e => e.Id != vehicle.Enterprise.Id))
            return Result.Fail<VehicleMileagePeriodReport>(ReportsHandlerErrors.ManagerNotAllowedToVehicle);

        Result<string> getSqlPeriodName = MapPeriodTypeToSqlPeriodName(query.Period);
        if (getSqlPeriodName.IsFailed)
            return getSqlPeriodName.ToResult<VehicleMileagePeriodReport>();

        object[] parameters = {
            new NpgsqlParameter("@vehicleId", vehicle.Id),
            new NpgsqlParameter("@startTime", query.StartDate.Value),
            new NpgsqlParameter("@endTime", query.EndDate.Value),
            new NpgsqlParameter("@period", getSqlPeriodName.Value)
        };

        List<VehicleMileageReportPeriodDto> dataDto = await DbContext.Database
            .SqlQueryRaw<VehicleMileageReportPeriodDto>(GetVehicleMileagePeriodSql, parameters)
            .ToListAsync();

        List<DataPeriodItem<VehicleMileageReportDataItem>> data = dataDto
            .Select(dto => new DataPeriodItem<VehicleMileageReportDataItem>(new UtcDateTimeOffset(dto.Date), new VehicleMileageReportDataItem(dto.MileageInKm)))
            .ToList();

        VehicleMileagePeriodReport report = new VehicleMileagePeriodReport(
            vehicle.Id,
            query.Period,
            query.StartDate,
            query.EndDate,
            data);

        return Result.Ok<VehicleMileagePeriodReport>(report);
    }

    private static Result<string> MapPeriodTypeToSqlPeriodName(PeriodType type)
    {
        return type switch
        {
            PeriodType.Day => "day",
            PeriodType.Month => "month",
            PeriodType.Year => "year",
            _ => Result.Fail(ReportsHandlerErrors.UnknownPeriodType)
        };
    }

    private class VehicleMileageReportPeriodDto
    {
        public DateTimeOffset Date { get; set; }
        public double MileageInKm { get; set; }
    }

    //public async Task<Result<EnterpriseRidesPeriodReport>> Handle(GetEnterpriseRidesReportQuery query)
    //{
    //    Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
    //    if (getManager.IsFailed)
    //        return getManager.ToResult<EnterpriseRidesPeriodReport>();

    //    Manager manager = getManager.Value;

    //    Enterprise? enterprise = await DbContext.Enterprises
    //        .FindAsync(query.EnterpriseId);

    //    if (enterprise == null)
    //        return Result.Fail<EnterpriseVehiclesModelsReport>(ReportsHandlerErrors.EnterpriseNotFound);

    //    if (manager.Enterprises.All(e => e.Id != enterprise.Id))
    //        return Result.Fail<EnterpriseVehiclesModelsReport>(ReportsHandlerErrors.ManagerNotAllowedToEnterprise);

    //    List<DataPeriodItem<EnterpriseRidesReportDataItem>> data =
    //        new List<DataPeriodItem<EnterpriseRidesReportDataItem>>();

    //    EnterpriseRidesPeriodReport report = new EnterpriseRidesPeriodReport(
    //        query.EnterpriseId,
    //        query.Period,
    //        query.StartDate,
    //        query.EndDate,
    //        data);

    //    return Result.Ok<EnterpriseRidesPeriodReport>(report);
    //}

    //public async Task<Result<EnterpriseVehiclesModelsReport>> Handle(GetEnterpriseModelsReportQuery query)
    //{
    //    Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
    //    if (getManager.IsFailed)
    //        return getManager.ToResult<EnterpriseVehiclesModelsReport>();

    //    Manager manager = getManager.Value;

    //    Enterprise? enterprise = await DbContext.Enterprises
    //        .FindAsync(query.EnterpriseId);

    //    if (enterprise == null)
    //        return Result.Fail<EnterpriseVehiclesModelsReport>(ReportsHandlerErrors.EnterpriseNotFound);

    //    if (manager.Enterprises.All(e => e.Id != enterprise.Id))
    //        return Result.Fail<EnterpriseVehiclesModelsReport>(ReportsHandlerErrors.ManagerNotAllowedToEnterprise);

    //    List<EnterpriseVehiclesModelsReportData> data = new List<EnterpriseVehiclesModelsReportData>();

    //    EnterpriseVehiclesModelsReport report = new EnterpriseVehiclesModelsReport(query.EnterpriseId, data);

    //    return Result.Ok<EnterpriseVehiclesModelsReport>(report);
    //}
}