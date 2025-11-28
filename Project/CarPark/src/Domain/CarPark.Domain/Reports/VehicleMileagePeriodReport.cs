using CarPark.DateTimes;
using CarPark.Reports.Abstract;

namespace CarPark.Reports;

public class VehicleMileagePeriodReport : BasePeriodReport<VehicleMileageReportDataItem>
{
    public Guid EnterpriseId { get; set; }

    public string EnterpriseName { get; set; }

    public Guid VehicleId { get; private init; }

    public string VehicleVinNumber { get; private init; }

    public VehicleMileagePeriodReport(
        Guid enterpriseId,
        string enterpriseName,
        Guid vehicleId,
        string vehicleVinNumber,
        PeriodType period, 
        UtcDateTimeOffset startDate, 
        UtcDateTimeOffset endDate, 
        IEnumerable<DataPeriodItem<VehicleMileageReportDataItem>> dataItems) 
        : base("VehicleMileage", period, startDate, endDate, dataItems)
    {
        EnterpriseId = enterpriseId;
        EnterpriseName = enterpriseName;
        VehicleId = vehicleId;
        VehicleVinNumber = vehicleVinNumber;
    }
}