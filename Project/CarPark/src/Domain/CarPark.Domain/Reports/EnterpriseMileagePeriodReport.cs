using CarPark.DateTimes;
using CarPark.Reports.Abstract;

namespace CarPark.Reports;

public class EnterpriseMileagePeriodReport : BasePeriodReport<EnterpriseMileageReportDataItem>
{
    public Guid EnterpriseId { get; private init; }

    public string EnterpriseName { get; private init; }

    public EnterpriseMileagePeriodReport(
        Guid enterpriseId,
        string enterpriseName,
        PeriodType period,
        UtcDateTimeOffset startDate,
        UtcDateTimeOffset endDate,
        IEnumerable<DataPeriodItem<EnterpriseMileageReportDataItem>> dataItems)
        : base("EnterpriseMileage", period, startDate, endDate, dataItems)
    {
        EnterpriseId = enterpriseId;
        EnterpriseName = enterpriseName;
    }
}