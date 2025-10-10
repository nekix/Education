using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.ExportImport.Queries;

public class ExportEnterpriseQuery : BaseManagerCommandQuery, IQuery<Result<EnterpriseExportImportDto>>
{
    public required int EnterpriseId { get; set; }
}