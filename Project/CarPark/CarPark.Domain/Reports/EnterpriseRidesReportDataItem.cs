namespace CarPark.Reports;

public class EnterpriseRidesReportDataItem
{
    public int RidesCount { get; private init; }

    public TimeSpan RidesTotalTime { get; private init; }

    public TimeSpan RidesAvgTime { get; private init; }

    public double RidesTotalMileageKm { get; private init; }

    public double RidesAvgMileageKm { get; private init; }

    public EnterpriseRidesReportDataItem(int ridesCount, 
        TimeSpan ridesTotalTime, 
        TimeSpan ridesAvgTime, 
        double ridesTotalMileageKm, 
        double ridesAvgMileageKm)
    {
        RidesCount = ridesCount;
        RidesTotalTime = ridesTotalTime;
        RidesAvgTime = ridesAvgTime;
        RidesTotalMileageKm = ridesTotalMileageKm;
        RidesAvgMileageKm = ridesAvgMileageKm;
    }
}