using CarPark.Identity;
using CarPark.ManagersOperations.Reports.Queries;
using CarPark.Reports;
using CarPark.Reports.Abstract;
using CarPark.Shared.CQ;
using CarPark.Shared.DateTimes;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class ReportsController : ApiBaseController
{
    private readonly IQueryHandler<GetVehicleMileageReportQuery, Result<VehicleMileagePeriodReport>> _reportsQueryHandler;

    public ReportsController(IQueryHandler<GetVehicleMileageReportQuery, Result<VehicleMileagePeriodReport>> reportsQueryHandler)
    {
        _reportsQueryHandler = reportsQueryHandler;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VehicleMileagePeriodReport))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        Result<VehicleMileagePeriodReport> result = await _reportsQueryHandler.Handle(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest();
    }

    //public async Task<IActionResult> GetEnterpriseRidesReport(Guid enterpriseId)
    //{
    //    Guid managerId = GetCurrentManagerId();

    //    GetEnterpriseRidesReportQuery query = new GetEnterpriseRidesReportQuery
    //    {
    //        RequestingManagerId = managerId,
    //        EnterpriseId = enterpriseId
    //    };

    //    Result<EnterpriseRidesPeriodReport> result = await _reportsQueryHandler.Handle(query);

    //    if (result.IsSuccess)
    //    {
    //        return Ok(result.Value);
    //    }
    //}

    //public async Task<IActionResult> GetEnterpriseModelsReport(Guid enterpriseId)
    //{
    //    Guid managerId = GetCurrentManagerId();

    //    GetEnterpriseModelsReportQuery query = new GetEnterpriseModelsReportQuery
    //    {
    //        RequestingManagerId = managerId,
    //        EnterpriseId = enterpriseId
    //    };

    //    Result<EnterpriseVehiclesModelsReport> result = await _reportsQueryHandler.Handle(query);

    //    if (result.IsSuccess)
    //    {
    //        return Ok(result.Value);
    //    }
    //}
}