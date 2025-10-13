namespace CarPark.Reports;

public class EnterpriseVehiclesModelsReportData
{
    public Guid ModelId { get; private init; }

    public int VehiclesCount { get; private init; }

    public EnterpriseVehiclesModelsReportData(Guid modelId, int vehiclesCount)
    {
        ModelId = modelId;
        VehiclesCount = vehiclesCount;
    }
}