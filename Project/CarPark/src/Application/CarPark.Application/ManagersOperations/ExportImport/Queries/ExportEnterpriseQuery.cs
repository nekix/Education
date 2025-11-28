using CarPark.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.ExportImport.Queries;

public class ExportEnterpriseQuery : BaseManagerCommandQuery, IQuery<Result<EnterpriseExportImportDto>>
{
    public required Guid EnterpriseId { get; set; }
}