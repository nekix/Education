namespace CarPark.Reports;

public class EnterpriseRidesReportDataItem
{
    public int ActiveRidesCount { get; private init; }

    public TimeSpan TotalTime { get; private init; }

    public TimeSpan AvgTime { get; private init; }

    public double TotalMileageKm { get; private init; }

    public double AvgMileageKm { get; private init; }

    public EnterpriseRidesReportDataItem(
        int activeRidesCount, 
        TimeSpan totalTime, 
        TimeSpan avgTime, 
        double totalMileageKm, 
        double avgMileageKm)
    {
        ActiveRidesCount = activeRidesCount;
        TotalTime = totalTime;
        AvgTime = avgTime;
        TotalMileageKm = totalMileageKm;
        AvgMileageKm = avgMileageKm;
    }

    private EnterpriseRidesReportDataItem()
    {

    }
}