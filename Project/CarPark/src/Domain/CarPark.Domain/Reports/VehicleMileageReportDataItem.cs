namespace CarPark.Reports;

public class VehicleMileageReportDataItem
{
    public double MileageInKm { get; private init; }

    public VehicleMileageReportDataItem(double mileageInKm)
    {
        MileageInKm = mileageInKm;
    }
}