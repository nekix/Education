using CarPark.CQ;
using CarPark.DateTimes;
using CarPark.ManagersOperations.Reports.Queries;
using CarPark.Reports;
using CarPark.Reports.Abstract;
using FluentResults;
using System.Text;

namespace CarPark.Telegram.Services;

internal interface ITelegramReportService
{
    Task<string> GetVehicleMileageReportAsync(Guid managerId, Guid vehicleId, UtcDateTimeOffset startDate, UtcDateTimeOffset endDate, PeriodType period);
    Task<string> GetEnterpriseMileageReportAsync(Guid managerId, Guid enterpriseId, UtcDateTimeOffset startDate, UtcDateTimeOffset endDate, PeriodType period);
}

internal sealed class TelegramReportService : ITelegramReportService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramReportService> _logger;

    public TelegramReportService(IServiceProvider serviceProvider, ILogger<TelegramReportService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<string> GetVehicleMileageReportAsync(Guid managerId, Guid vehicleId, UtcDateTimeOffset startDate, UtcDateTimeOffset endDate, PeriodType period)
    {
        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();

            IQueryHandler<GetVehicleMileageReportQuery, Result<VehicleMileagePeriodReport>> queryHandler =
                scope.ServiceProvider.GetRequiredService<IQueryHandler<GetVehicleMileageReportQuery, Result<VehicleMileagePeriodReport>>>();

            GetVehicleMileageReportQuery query = new GetVehicleMileageReportQuery
            {
                RequestingManagerId = managerId,
                VehicleId = vehicleId,
                Period = period,
                StartDate = startDate,
                EndDate = endDate
            };

            Result<VehicleMileagePeriodReport> result = await queryHandler.Handle(query);

            if (result.IsFailed)
            {
                return $"❌ Ошибка при получении отчета по автомобилю: {string.Join(", ", result.Errors.Select(e => e.Message))}";
            }

            VehicleMileagePeriodReport report = result.Value;

            StringBuilder sb = new();
            sb.AppendLine($"🚗 <b>Отчет по автомобилю</b>");
            sb.AppendLine($"📍 Предприятие: {report.EnterpriseName}");
            sb.AppendLine($"🔢 VIN: {report.VehicleVinNumber}");
            sb.AppendLine($"📅 Период: {report.StartDate.Value:dd.MM.yyyy HH:mm:ss UTC} - {report.EndDate.Value:dd.MM.yyyy HH:mm:ss UTC}");
            sb.AppendLine();

            if (!report.DataItems.Any())
            {
                sb.AppendLine("📊 Данных за указанный период не найдено");
                return sb.ToString();
            }

            foreach (DataPeriodItem<VehicleMileageReportDataItem> item in report.DataItems)
            {
                sb.AppendLine($"📈 {item.Date.Value:dd.MM.yyyy HH:mm:ss UTC}: <b>{item.Data.MileageInKm:F1} км</b>");
            }

            double totalMileage = report.DataItems.Sum(item => item.Data.MileageInKm);
            sb.AppendLine();
            sb.AppendLine($"🛣️ <b>Общий пробег: {totalMileage:F1} км</b>");

            return sb.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vehicle mileage report for manager {ManagerId}, vehicle {VehicleId}", managerId, vehicleId);
            return "❌ Произошла ошибка при получении отчета по автомобилю";
        }
    }

    public async Task<string> GetEnterpriseMileageReportAsync(Guid managerId, Guid enterpriseId, UtcDateTimeOffset startDate, UtcDateTimeOffset endDate, PeriodType period)
    {
        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();

            IQueryHandler<GetEnterpriseMileageReportQuery, Result<EnterpriseMileagePeriodReport>> queryHandler =
                scope.ServiceProvider.GetRequiredService<IQueryHandler<GetEnterpriseMileageReportQuery, Result<EnterpriseMileagePeriodReport>>>();

            GetEnterpriseMileageReportQuery query = new GetEnterpriseMileageReportQuery
            {
                RequestingManagerId = managerId,
                EnterpriseId = enterpriseId,
                Period = period,
                StartDate = startDate,
                EndDate = endDate
            };

            Result<EnterpriseMileagePeriodReport> result = await queryHandler.Handle(query);

            if (result.IsFailed)
            {
                return $"❌ Ошибка при получении отчета по пробегу предприятия: {string.Join(", ", result.Errors.Select(e => e.Message))}";
            }

            EnterpriseMileagePeriodReport report = result.Value;

            StringBuilder sb = new();
            sb.AppendLine($"🏢 <b>Отчет по пробегу предприятия</b>");
            sb.AppendLine($"📍 Предприятие: {report.EnterpriseName}");
            sb.AppendLine($"📅 Период: {report.StartDate.Value:dd.MM.yyyy HH:mm:ss UTC} - {report.EndDate.Value:dd.MM.yyyy HH:mm:ss UTC}");
            sb.AppendLine();

            if (!report.DataItems.Any())
            {
                sb.AppendLine("📊 Данных за указанный период не найдено");
                return sb.ToString();
            }

            foreach (DataPeriodItem<EnterpriseMileageReportDataItem> item in report.DataItems)
            {
                sb.AppendLine($"📈 {item.Date.Value:dd.MM.yyyy HH:mm:ss UTC}:");
                sb.AppendLine($"   🛣️ Общий пробег: {item.Data.TotalMileageKm:F1} км");
                sb.AppendLine($"   📏 Средний пробег: {item.Data.AvgMileageKm:F1} км");
                sb.AppendLine();
            }

            double totalMileage = report.DataItems.Sum(item => item.Data.TotalMileageKm);
            double avgMileage = report.DataItems.Average(item => item.Data.AvgMileageKm);

            sb.AppendLine($"📊 <b>Итого за период:</b>");
            sb.AppendLine($"🛣️ Общий пробег: {totalMileage:F1} км");
            sb.AppendLine($"📏 Средний пробег: {avgMileage:F1} км");

            return sb.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting enterprise mileage report for manager {ManagerId}, enterprise {EnterpriseId}", managerId, enterpriseId);
            return "❌ Произошла ошибка при получении отчета по пробегу предприятия";
        }
    }
}