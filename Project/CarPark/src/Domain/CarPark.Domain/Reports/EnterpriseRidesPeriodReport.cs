using CarPark.DateTimes;
using CarPark.Reports.Abstract;

namespace CarPark.Reports;

public class EnterpriseRidesPeriodReport : BasePeriodReport<EnterpriseRidesReportDataItem>
{
    public Guid EnterpriseId { get; private init; }

    public string EnterpriseName { get; private init; }

    public EnterpriseRidesPeriodReport(
        Guid enterpriseId,
        string enterpriseName,
        PeriodType period, 
        UtcDateTimeOffset startDate, 
        UtcDateTimeOffset endDate, 
        IEnumerable<DataPeriodItem<EnterpriseRidesReportDataItem>> dataItems) 
        : base("EnterpriseRides", period, startDate, endDate, dataItems)
    {
        EnterpriseId = enterpriseId;
        EnterpriseName = enterpriseName;
    }
}