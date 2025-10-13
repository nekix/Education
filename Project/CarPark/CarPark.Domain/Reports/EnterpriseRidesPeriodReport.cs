using CarPark.Reports.Abstract;
using CarPark.Shared.DateTimes;

namespace CarPark.Reports;

public class EnterpriseRidesPeriodReport : BasePeriodReport<EnterpriseRidesReportDataItem>
{
    public Guid EnterpriseId { get; private init; }

    public EnterpriseRidesPeriodReport(
        Guid enterpriseId, 
        PeriodType period, 
        UtcDateTimeOffset startDate, 
        UtcDateTimeOffset endDate, 
        IEnumerable<DataPeriodItem<EnterpriseRidesReportDataItem>> dataItems) 
        : base("EnterpriseRides", period, startDate, endDate, dataItems)
    {
        EnterpriseId = enterpriseId;
    }
}