using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.ExportImport.Queries;

public class ExportModelsQuery : BaseManagerCommandQuery, IQuery<Result<List<ModelExportImportDto>>>
{
}