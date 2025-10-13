using CarPark.Reports.Abstract;

namespace CarPark.Reports;

public class EnterpriseVehiclesModelsReport : BaseReport<IReadOnlyList<EnterpriseVehiclesModelsReportData>>
{
    public Guid EnterpriseId { get; private init; }

    public EnterpriseVehiclesModelsReport(Guid enterpriseId,
        IEnumerable<EnterpriseVehiclesModelsReportData> data) 
        : base("EnterpriseVehiclesModelsReport", data.ToList())
    {
        EnterpriseId = enterpriseId;
    }
}