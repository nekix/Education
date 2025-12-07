using CarPark.CQ;
using CarPark.Data;
using CarPark.DateTimes;
using CarPark.Enterprises;
using CarPark.Managers;
using CarPark.Reports;
using CarPark.Reports.Abstract;
using CarPark.Vehicles;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CarPark.ManagersOperations.Reports.Queries;

public class ReportsQueryHandler : BaseManagersHandler,
    IQueryHandler<GetVehicleMileageReportQuery, Result<VehicleMileagePeriodReport>>,
    IQueryHandler<GetEnterpriseRidesReportQuery, Result<EnterpriseRidesPeriodReport>>,
    IQueryHandler<GetEnterpriseMileageReportQuery, Result<EnterpriseMileagePeriodReport>>,
    IQueryHandler<GetEnterpriseModelsReportQuery, Result<EnterpriseVehiclesModelsReport>>
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

    private const string GetEnterpriseRidesPeriodSql =
        @"
        WITH params AS (
            SELECT @period AS interval_type
        ),
         rides AS (
             SELECT
                 r.id AS ride_id,
                 r.vehicle_id,
                 r.start_time,
                 r.end_time,
                 sp.time AS start_point_time,
                 ep.time AS end_point_time
             FROM ride r
             JOIN car_park.vehicle v ON v.id = r.vehicle_id
             JOIN vehicle_geo_time_point sp
                  ON sp.id = r.start_geo_time_point_id
             JOIN vehicle_geo_time_point ep
                  ON ep.id = r.end_geo_time_point_id
             WHERE v.enterprise_id = @enterpriseId
               AND r.start_time >= @startTime
               AND r.end_time <= @endTime
         ),
         periods AS (
             SELECT
                 r.ride_id,
                 r.vehicle_id,
                 r.start_time,
                 r.end_time,
                 r.start_point_time,
                 r.end_point_time,
                 gs.period_start,
                 CASE p.interval_type
                     WHEN 'day' THEN gs.period_start + interval '1 day'
                     WHEN 'month' THEN gs.period_start + interval '1 month'
                     WHEN 'year' THEN gs.period_start + interval '1 year'
                     END AS period_end,
                 GREATEST(r.start_time, gs.period_start) AS effective_start,
                 LEAST(r.end_time,
                       CASE p.interval_type
                           WHEN 'day' THEN gs.period_start + interval '1 day'
                           WHEN 'month' THEN gs.period_start + interval '1 month'
                           WHEN 'year' THEN gs.period_start + interval '1 year'
                           END
                 ) AS effective_end,
                 GREATEST(r.start_point_time, gs.period_start) AS gps_start,
                 LEAST(r.end_point_time,
                       CASE p.interval_type
                           WHEN 'day' THEN gs.period_start + interval '1 day'
                           WHEN 'month' THEN gs.period_start + interval '1 month'
                           WHEN 'year' THEN gs.period_start + interval '1 year'
                           END
                 ) AS gps_end
             FROM rides r
                      CROSS JOIN params p
                      CROSS JOIN LATERAL (
                 SELECT generate_series(
                                DATE_TRUNC(p.interval_type, r.start_time),
                                DATE_TRUNC(p.interval_type, r.end_time),
                                CASE p.interval_type
                                    WHEN 'day' THEN interval '1 day'
                                    WHEN 'month' THEN interval '1 month'
                                    WHEN 'year' THEN interval '1 year'
                                    END
                        ) AS period_start
                 ) gs
             WHERE r.start_time < (
                 CASE p.interval_type
                     WHEN 'day' THEN gs.period_start + interval '1 day'
                     WHEN 'month' THEN gs.period_start + interval '1 month'
                     WHEN 'year' THEN gs.period_start + interval '1 year'
                     END)
               AND r.end_time > gs.period_start
         ),
         segments AS (
             SELECT
                 pr.ride_id,
                 pr.vehicle_id,
                 pr.period_start,
                 pr.period_end,
                 p.time AS seg_start_time,
                 LEAD(p.time) OVER (PARTITION BY pr.ride_id, pr.period_start ORDER BY p.time) AS seg_end_time,
                 p.location AS seg_start_loc,
                 LEAD(p.location) OVER (PARTITION BY pr.ride_id, pr.period_start ORDER BY p.time) AS seg_end_loc
             FROM periods pr
                      JOIN vehicle_geo_time_point p
                           ON p.vehicle_id = pr.vehicle_id
                               AND p.time BETWEEN pr.gps_start AND pr.gps_end
         ),
         segments_with_distance AS (
             SELECT
                 ride_id,
                 vehicle_id,
                 period_start,
                 period_end,
                 ST_Distance(seg_start_loc::geography, seg_end_loc::geography) AS segment_distance_m
             FROM segments
             WHERE seg_end_time IS NOT NULL
         ),
         ride_metrics AS (
             SELECT
                 pr.ride_id,
                 pr.period_start,
                 (pr.effective_end - pr.effective_start) AS ride_time_in_period,
                 COALESCE(SUM(s.segment_distance_m), 0) AS distance_in_period_m
             FROM periods pr
                      LEFT JOIN segments_with_distance s
                                ON s.ride_id = pr.ride_id
                                    AND s.period_start = pr.period_start
             GROUP BY pr.ride_id, pr.period_start, pr.effective_start, pr.effective_end
         )
        SELECT
            DATE_TRUNC(@period, period_start)::timestamptz AS date,
            COUNT(DISTINCT ride_id) AS active_rides_count,
            make_interval(secs => SUM(EXTRACT(EPOCH FROM ride_time_in_period))) AS total_time,
            make_interval(secs => AVG(EXTRACT(EPOCH FROM ride_time_in_period))) AS avg_time,
            SUM(distance_in_period_m) / 1000.0 AS total_mileage_km,
            AVG(distance_in_period_m) / 1000.0 AS avg_mileage_km
        FROM ride_metrics
        GROUP BY date
        ORDER BY date;
        ";

   private const string GetEnterpriseMileagePeriodSql =
       @"
       WITH distances AS (
           SELECT
               v.id AS vehicle_id,
               p.time,
               ST_Distance(
                   p.location::geography,
                   LAG(p.location) OVER (PARTITION BY v.id ORDER BY p.time)::geography
               ) AS segment_distance,
               EXTRACT(EPOCH FROM (p.time - LAG(p.time) OVER (PARTITION BY v.id ORDER BY p.time))) / 60 AS diff_minutes
           FROM car_park.vehicle_geo_time_point p
           JOIN car_park.vehicle v ON v.id = p.vehicle_id
           WHERE v.enterprise_id = @enterpriseId
             AND p.time BETWEEN @startTime AND @endTime
       ),
       vehicle_periods AS (
           SELECT
               vehicle_id,
               DATE_TRUNC(@period, time)::timestamptz AS date,
               SUM(segment_distance)/1000.0 AS mileage_in_km
           FROM distances
           WHERE diff_minutes <= 10
           GROUP BY vehicle_id, date
       )
       SELECT
           date,
           SUM(mileage_in_km) AS total_mileage_km,
           AVG(mileage_in_km) AS avg_mileage_km
       FROM vehicle_periods
       GROUP BY date
       ORDER BY date;
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
            vehicle.Enterprise.Id,
            vehicle.Enterprise.Name,
            vehicle.Id,
            vehicle.VinNumber,
            query.Period,
            query.StartDate,
            query.EndDate,
            data);

        return Result.Ok<VehicleMileagePeriodReport>(report);
    }

    private class VehicleMileageReportPeriodDto
    {
        public DateTimeOffset Date { get; set; }
        public double MileageInKm { get; set; }
    }

    public async Task<Result<EnterpriseRidesPeriodReport>> Handle(GetEnterpriseRidesReportQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<EnterpriseRidesPeriodReport>();

        Manager manager = getManager.Value;

        Enterprise? enterprise = await DbContext.Enterprises
            .FindAsync(query.EnterpriseId);

        if (enterprise == null)
            return Result.Fail<EnterpriseRidesPeriodReport>(ReportsHandlerErrors.EnterpriseNotFound);

        if (manager.Enterprises.All(e => e.Id != enterprise.Id))
            return Result.Fail<EnterpriseRidesPeriodReport>(ReportsHandlerErrors.ManagerNotAllowedToEnterprise);

        Result<string> getSqlPeriodName = MapPeriodTypeToSqlPeriodName(query.Period);
        if (getSqlPeriodName.IsFailed)
            return getSqlPeriodName.ToResult<EnterpriseRidesPeriodReport>();

        object[] parameters = {
            new NpgsqlParameter("@enterpriseId", enterprise.Id),
            new NpgsqlParameter("@startTime", query.StartDate.Value),
            new NpgsqlParameter("@endTime", query.EndDate.Value),
            new NpgsqlParameter("@period", getSqlPeriodName.Value)
        };

        List<EnterpriseRidesReportDataDto> dataDto = await DbContext.Database
            .SqlQueryRaw<EnterpriseRidesReportDataDto>(GetEnterpriseRidesPeriodSql, parameters)
            .ToListAsync();

        List<DataPeriodItem<EnterpriseRidesReportDataItem>> data = dataDto
            .Select(dto => new DataPeriodItem<EnterpriseRidesReportDataItem>(
                new UtcDateTimeOffset(dto.Date),
                new EnterpriseRidesReportDataItem(
                    dto.ActiveRidesCount, dto.TotalTime, dto.AvgTime, dto.TotalMileageKm, dto.AvgMileageKm)))
            .ToList();

        EnterpriseRidesPeriodReport report = new EnterpriseRidesPeriodReport(
            enterprise.Id,
            enterprise.Name,
            query.Period,
            query.StartDate,
            query.EndDate,
            data);

        return Result.Ok<EnterpriseRidesPeriodReport>(report);
    }

    private class EnterpriseRidesReportDataDto
    {
        public DateTimeOffset Date { get; set; }

        public int ActiveRidesCount { get; private init; }

        public TimeSpan TotalTime { get; private init; }

        public TimeSpan AvgTime { get; private init; }

        public double TotalMileageKm { get; private init; }

        public double AvgMileageKm { get; private init; }
    }

    public async Task<Result<EnterpriseMileagePeriodReport>> Handle(GetEnterpriseMileageReportQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<EnterpriseMileagePeriodReport>();

        Manager manager = getManager.Value;

        Enterprise? enterprise = await DbContext.Enterprises
            .FindAsync(query.EnterpriseId);

        if (enterprise == null)
            return Result.Fail<EnterpriseMileagePeriodReport>(ReportsHandlerErrors.EnterpriseNotFound);

        if (manager.Enterprises.All(e => e.Id != enterprise.Id))
            return Result.Fail<EnterpriseMileagePeriodReport>(ReportsHandlerErrors.ManagerNotAllowedToEnterprise);

        Result<string> getSqlPeriodName = MapPeriodTypeToSqlPeriodName(query.Period);
        if (getSqlPeriodName.IsFailed)
            return getSqlPeriodName.ToResult<EnterpriseMileagePeriodReport>();

        object[] parameters = {
            new NpgsqlParameter("@enterpriseId", enterprise.Id),
            new NpgsqlParameter("@startTime", query.StartDate.Value),
            new NpgsqlParameter("@endTime", query.EndDate.Value),
            new NpgsqlParameter("@period", getSqlPeriodName.Value)
        };

        List<EnterpriseMileageReportDataDto> dataDto = await DbContext.Database
            .SqlQueryRaw<EnterpriseMileageReportDataDto>(GetEnterpriseMileagePeriodSql, parameters)
            .ToListAsync();

        List<DataPeriodItem<EnterpriseMileageReportDataItem>> data = dataDto
            .Select(dto => new DataPeriodItem<EnterpriseMileageReportDataItem>(
                new UtcDateTimeOffset(dto.Date),
                new EnterpriseMileageReportDataItem(
                    dto.TotalMileageKm, dto.AvgMileageKm)))
            .ToList();

        EnterpriseMileagePeriodReport report = new EnterpriseMileagePeriodReport(
            enterprise.Id,
            enterprise.Name,
            query.Period,
            query.StartDate,
            query.EndDate,
            data);

        return Result.Ok<EnterpriseMileagePeriodReport>(report);
    }

    private class EnterpriseMileageReportDataDto
    {
        public DateTimeOffset Date { get; set; }

        public double TotalMileageKm { get; private init; }

        public double AvgMileageKm { get; private init; }
    }

    public async Task<Result<EnterpriseVehiclesModelsReport>> Handle(GetEnterpriseModelsReportQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<EnterpriseVehiclesModelsReport>();

        Manager manager = getManager.Value;

        Enterprise? enterprise = await DbContext.Enterprises
            .FindAsync(query.EnterpriseId);

        if (enterprise == null)
            return Result.Fail<EnterpriseVehiclesModelsReport>(ReportsHandlerErrors.EnterpriseNotFound);

        if (manager.Enterprises.All(e => e.Id != enterprise.Id))
            return Result.Fail<EnterpriseVehiclesModelsReport>(ReportsHandlerErrors.ManagerNotAllowedToEnterprise);

        List<EnterpriseVehiclesModelsReportData> data = DbContext.Vehicles
            .Where(v => v.Enterprise.Id == enterprise.Id)
            .GroupBy(v => new { v.Model.Id, v.Model.ModelName })
            .Select(g => new
            {
                ModelId = g.Key.Id,
                ModelName = g.Key.ModelName,
                VehiclesCount = g.Count()
            })
            .OrderByDescending(dto => dto.VehiclesCount)
            .ToList()
            .Select(dto => new EnterpriseVehiclesModelsReportData(dto.ModelId, dto.ModelName, dto.VehiclesCount))
            .ToList();

        EnterpriseVehiclesModelsReport report = new EnterpriseVehiclesModelsReport(
            enterprise.Id,
            enterprise.Name,
            data);

        return Result.Ok<EnterpriseVehiclesModelsReport>(report);
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
}