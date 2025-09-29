using CarPark.Controllers.Api.Abstract;
using CarPark.Identity;
using CarPark.ManagersOperations;
using CarPark.ManagersOperations.Drivers;
using CarPark.ManagersOperations.Drivers.Queries;
using CarPark.ManagersOperations.Drivers.Queries.Models;
using CarPark.Shared.CQ;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        int managerId = GetCurrentManagerId();

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

        if (getDrivers.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    // GET: api/Drivers/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DriverDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DriverDto>> GetDriver(int id)
    {
        int managerId = GetCurrentManagerId();


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

        if (getDriver.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (getDriver.HasError(e => e.Message == DriversHandlerErrors.DriverNotExist))
        {
            return NotFound();
        }
        else if (getDriver.HasError(e => e.Message == DriversHandlerErrors.ManagerNotAllowedToVehicle))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
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