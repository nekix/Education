using CarPark.Controllers.Api.Abstract;
using CarPark.Errors;
using CarPark.Identity;
using CarPark.ManagersOperations.Drivers.Queries;
using CarPark.ManagersOperations.Drivers.Queries.Models;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using CarPark.CQ;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class DriversController : ApiBaseController
{
    private readonly IQueryHandler<GetDriverQuery, Result<DriverDto>> _getDriverQueryHandler;
    private readonly IQueryHandler<GetDriversListQuery, Result<PaginatedDrivers>> _getDriversListQueryHandler;

    public DriversController(IQueryHandler<GetDriverQuery, Result<DriverDto>> getDriverQueryHandler, 
        IQueryHandler<GetDriversListQuery, Result<PaginatedDrivers>> getDriversListQueryHandler)
    {
        _getDriverQueryHandler = getDriverQueryHandler;
        _getDriversListQueryHandler = getDriversListQueryHandler;
    }

    // GET: api/Drivers
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedDrivers))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedDrivers>> GetDrivers([FromQuery] GetDriversRequest request)
    {
        Guid managerId = GetCurrentManagerId();

        GetDriversListQuery query = new GetDriversListQuery
        {
            RequestingManagerId = managerId,
            Limit = request.Limit,
            Offset = request.Offset
        };

        Result<PaginatedDrivers> getDrivers = await _getDriversListQueryHandler.Handle(query);

        if (getDrivers.IsSuccess)
        {
            return Ok(getDrivers.Value);
        }

        WebApiError? apiError = getDrivers.Errors.OfType<WebApiError>().FirstOrDefault();
        if (apiError != null)
        {
            return StatusCode(apiError.StatusCode, new { message = apiError.UserMessage });
        }

        return BadRequest();
    }

    // GET: api/Drivers/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DriverDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DriverDto>> GetDriver(Guid id)
    {
        Guid managerId = GetCurrentManagerId();

        GetDriverQuery query = new GetDriverQuery
        {
            RequestingManagerId = managerId,
            DriverId = id
        };

        Result<DriverDto> getDriver = await _getDriverQueryHandler.Handle(query);

        if (getDriver.IsSuccess)
        {
            return Ok(getDriver.Value);
        }

        WebApiError? apiError = getDriver.Errors.OfType<WebApiError>().FirstOrDefault();
        if (apiError != null)
        {
            return StatusCode(apiError.StatusCode, new { message = apiError.UserMessage });
        }

        return BadRequest();
    }

    public class GetDriversRequest : IPaginationRequest
    {
        [Required]
        [Range(1, 1000)]
        public required uint Limit { get; init; }

        [Required]
        [Range(0, int.MaxValue)]
        public required uint Offset { get; init; }
    }
}