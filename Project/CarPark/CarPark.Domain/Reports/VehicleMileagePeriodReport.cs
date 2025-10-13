using CarPark.Reports.Abstract;
using CarPark.Shared.DateTimes;

namespace CarPark.Reports;

public class VehicleMileagePeriodReport : BasePeriodReport<VehicleMileageReportDataItem>
{
    public Guid VehicleId { get; private init; }

    public VehicleMileagePeriodReport(
        Guid vehicleId,
        PeriodType period, 
        UtcDateTimeOffset startDate, 
        UtcDateTimeOffset endDate, 
        IEnumerable<DataPeriodItem<VehicleMileageReportDataItem>> dataItems) 
        : base("VehicleMileage", period, startDate, endDate, dataItems)
    {
        VehicleId = vehicleId;
    }
}