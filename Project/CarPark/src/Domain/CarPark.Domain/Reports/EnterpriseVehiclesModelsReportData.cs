namespace CarPark.Reports;

public class EnterpriseVehiclesModelsReportData
{
    public Guid ModelId { get; private init; }

    public string ModelName { get; private init; }

    public int VehiclesCount { get; private init; }

    public EnterpriseVehiclesModelsReportData(Guid modelId, string modelName, int vehiclesCount)
    {
        ModelId = modelId;
        ModelName = modelName;
        VehiclesCount = vehiclesCount;
    }
}