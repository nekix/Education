using CarPark.Identity;
using CarPark.ManagersOperations.Reports;
using CarPark.ManagersOperations.Reports.Queries;
using CarPark.Reports;
using CarPark.Reports.Abstract;
using CarPark.Shared.CQ;
using CarPark.Shared.DateTimes;
using ClosedXML.Report;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class ReportsController : ApiBaseController
{
    private const string XLSX_ACCEPT_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly IQueryHandler<GetVehicleMileageReportQuery, Result<VehicleMileagePeriodReport>> _getVehicleMileageQueryHandler;
    private readonly IQueryHandler<GetEnterpriseRidesReportQuery, Result<EnterpriseRidesPeriodReport>> _getEnterpriseRidesQueryHandler;
    private readonly IQueryHandler<GetEnterpriseModelsReportQuery, Result<EnterpriseVehiclesModelsReport>> _getEnterpriseModelsQueryHandler;

    public ReportsController(IQueryHandler<GetVehicleMileageReportQuery, Result<VehicleMileagePeriodReport>> getVehicleMileageQueryHandler,
        IQueryHandler<GetEnterpriseRidesReportQuery, Result<EnterpriseRidesPeriodReport>> getEnterpriseRidesQueryHandler,
        IQueryHandler<GetEnterpriseModelsReportQuery, Result<EnterpriseVehiclesModelsReport>> getEnterpriseModelsQueryHandler)
    {
        _getVehicleMileageQueryHandler = getVehicleMileageQueryHandler;
        _getEnterpriseRidesQueryHandler = getEnterpriseRidesQueryHandler;
        _getEnterpriseModelsQueryHandler = getEnterpriseModelsQueryHandler;
    }

    [HttpGet("vehicle-mileage")]
    [Produces("application/json", XLSX_ACCEPT_TYPE)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VehicleMileagePeriodReport))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetVehicleMileageReport(Guid vehicleId, DateTimeOffset startDate, DateTimeOffset endDate, PeriodType period)
    {
        Guid managerId = GetCurrentManagerId();

        GetVehicleMileageReportQuery query = new GetVehicleMileageReportQuery
        {
            RequestingManagerId = managerId,
            VehicleId = vehicleId,
            Period = period,
            StartDate = new UtcDateTimeOffset(startDate),
            EndDate = new UtcDateTimeOffset(endDate)
        };

        Result<VehicleMileagePeriodReport> result = await _getVehicleMileageQueryHandler.Handle(query);

        if (result.IsSuccess)
        {
            if (IsXlsxRequested())
            {
                return await XlsxReportFile("VehicleMileageReport",
                    result.Value,
                    @".\Reports\Xlsx\Templates\VehicleMileageReportTemplate.xlsx");
            }

            return Ok(result.Value);
        }

        if (result.HasError(e => e.Message == ReportsHandlerErrors.VehicleNotFound))
        {
            return NotFound();
        }

        if (result.HasError(e => e.Message == ReportsHandlerErrors.ManagerNotAllowedToVehicle))
        {
            return Forbid();
        }

        if (result.HasError(e => e.Message == ReportsHandlerErrors.UnknownPeriodType))
        {
            return BadRequest("Unknown period type");
        }

        return StatusCode(500);
    }

    [HttpGet("enterprise-rides")]
    [Produces("application/json", XLSX_ACCEPT_TYPE)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnterpriseRidesPeriodReport))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEnterpriseRidesReport(Guid enterpriseId, DateTimeOffset startDate, DateTimeOffset endDate, PeriodType period)
    {
        Guid managerId = GetCurrentManagerId();

        GetEnterpriseRidesReportQuery query = new GetEnterpriseRidesReportQuery
        {
            StartDate = new UtcDateTimeOffset(startDate),
            EndDate = new UtcDateTimeOffset(endDate),
            Period = period,
            RequestingManagerId = managerId,
            EnterpriseId = enterpriseId
        };

        Result<EnterpriseRidesPeriodReport> result = await _getEnterpriseRidesQueryHandler.Handle(query);

        if (result.IsSuccess)
        {
            if (IsXlsxRequested())
            {
                return await XlsxReportFile("EnterpriseRidesReport",
                    result.Value,
                    @".\Reports\Xlsx\Templates\EnterpriseRidesReportTemplate.xlsx");
            }

            return Ok(result.Value);
        }

        if (result.HasError(e => e.Message == ReportsHandlerErrors.EnterpriseNotFound))
        {
            return NotFound();
        }

        if (result.HasError(e => e.Message == ReportsHandlerErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }

        if (result.HasError(e => e.Message == ReportsHandlerErrors.UnknownPeriodType))
        {
            return BadRequest("Unknown period type");
        }

        return StatusCode(500);
    }

    [HttpGet("enterprise-models")]
    [Produces("application/json", XLSX_ACCEPT_TYPE)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnterpriseVehiclesModelsReport))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEnterpriseModelsReport(Guid enterpriseId)
    {
        Guid managerId = GetCurrentManagerId();

        GetEnterpriseModelsReportQuery query = new GetEnterpriseModelsReportQuery
        {
            RequestingManagerId = managerId,
            EnterpriseId = enterpriseId
        };

        Result<EnterpriseVehiclesModelsReport> result = await _getEnterpriseModelsQueryHandler.Handle(query);

        if (result.IsSuccess)
        {
            if (IsXlsxRequested())
            {
                return await XlsxReportFile("EnterpriseModelsReport", 
                    result.Value,
                    @".\Reports\Xlsx\Templates\EnterpriseModelsReportTemplate.xlsx");
            }

            return Ok(result.Value);
        }

        if (result.HasError(e => e.Message == ReportsHandlerErrors.EnterpriseNotFound))
        {
            return NotFound();
        }

        if (result.HasError(e => e.Message == ReportsHandlerErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }

        return StatusCode(500);
    }

    private bool IsXlsxRequested()
    {
        IList<MediaTypeHeaderValue> acceptHeader = Request.GetTypedHeaders().Accept;

        if (acceptHeader.Count == 0)
            return false;

        bool result = acceptHeader.MaxBy(v => v.Quality)?.MediaType.Equals(XLSX_ACCEPT_TYPE, StringComparison.OrdinalIgnoreCase) == true;

        return result;
    }

    // Helper methods for file export
    private static string GenerateFileName(string entityType, string format)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return $"{entityType}_report_{timestamp}.{format}";
    }

    private async Task<FileStreamResult> XlsxReportFile<T>(string reportName, T data, string templateFilePath)
    {
        XLTemplate template = new XLTemplate(templateFilePath);

        template.AddVariable(data);
        template.Generate();

        MemoryStream stream = new MemoryStream();
        template.SaveAs(stream, true);
        await stream.FlushAsync();
        stream.Position = 0;

        string fileName = GenerateFileName(reportName, "xlsx");

        return File(stream, XLSX_ACCEPT_TYPE, fileName);
    }
}