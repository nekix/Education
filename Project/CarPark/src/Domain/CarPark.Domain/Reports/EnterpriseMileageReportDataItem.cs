namespace CarPark.Reports;

public class EnterpriseMileageReportDataItem
{
    public double TotalMileageKm { get; private init; }

    public double AvgMileageKm { get; private init; }

    public EnterpriseMileageReportDataItem(
        double totalMileageKm,
        double avgMileageKm)
    {
        TotalMileageKm = totalMileageKm;
        AvgMileageKm = avgMileageKm;
    }

    private EnterpriseMileageReportDataItem()
    {

    }
}