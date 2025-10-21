using CarPark.Reports.Abstract;

namespace CarPark.Reports;

public class EnterpriseVehiclesModelsReport : BaseReport<IReadOnlyList<EnterpriseVehiclesModelsReportData>>
{
    public Guid EnterpriseId { get; private init; }

    public string EnterpriseName { get; private init; }

    public EnterpriseVehiclesModelsReport(
        Guid enterpriseId,
        string enterpriseName,
        IEnumerable<EnterpriseVehiclesModelsReportData> data) 
        : base("EnterpriseVehiclesModelsReport", data.ToList())
    {
        EnterpriseId = enterpriseId;
        EnterpriseName = enterpriseName;
    }
}