using CarPark.Attributes;
using CarPark.Controllers.Api.Abstract;
using CarPark.Identity;
using CarPark.ManagersOperations;
using CarPark.ManagersOperations.Rides;
using CarPark.ManagersOperations.Rides.Queries;
using CarPark.ManagersOperations.Tracks;
using CarPark.ManagersOperations.Tracks.Queries;
using CarPark.ManagersOperations.Tracks.Queries.Models;
using CarPark.ManagersOperations.Vehicles;
using CarPark.ManagersOperations.Vehicles.Commands;
using CarPark.ManagersOperations.Vehicles.Queries;
using CarPark.ManagersOperations.Vehicles.Queries.Models;
using CarPark.Shared.CQ;
using CarPark.Vehicles;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class VehiclesController : ApiBaseController
{
    private readonly ICommandHandler<CreateVehicleCommand, Result<Guid>> _createVehicleHandler;
    private readonly ICommandHandler<UpdateVehicleCommand, Result<Guid>> _updateVehicleHandler;
    private readonly ICommandHandler<DeleteVehicleCommand, Result> _deleteVehicleHandler;

    private readonly IQueryHandler<GetVehicleQuery, Result<VehicleDto>> _getVehicleQueryHandler;
    private readonly IQueryHandler<GetVehiclesListQuery, Result<PaginatedVehicles>> _getVehiclesListQueryHandler;

    private readonly IQueryHandler<GetTrackQuery, Result<TrackViewModel>> _getTrackQueryHandler;
    private readonly IQueryHandler<GetTrackFeatureCollectionQuery, Result<FeatureCollection>> _getTrackFeatureCollectionQueryHandler;
    private readonly IQueryHandler<GetRidesTrackQuery, Result<TrackViewModel>> _getRidesTrackQueryHandler;
    private readonly IQueryHandler<GetRidesTrackFeatureCollectionQuery, Result<FeatureCollection>> _getRidesTrackFeatureCollectionHandler;

    private readonly IQueryHandler<GetRidesQuery, Result<RidesViewModel>> _getRidesQueryHandler;

    public VehiclesController(
        IQueryHandler<GetVehicleQuery, Result<VehicleDto>> getVehicleQueryHandler,
        IQueryHandler<GetVehiclesListQuery, Result<PaginatedVehicles>> getVehiclesListQueryHandler,
        ICommandHandler<CreateVehicleCommand, Result<Guid>> createVehicleHandler,
        ICommandHandler<UpdateVehicleCommand, Result<Guid>> updateVehicleHandler,
        ICommandHandler<DeleteVehicleCommand, Result> deleteVehicleHandler,
        IQueryHandler<GetTrackQuery, Result<TrackViewModel>> getTrackQueryHandler,
        IQueryHandler<GetTrackFeatureCollectionQuery, Result<FeatureCollection>> getTrackFeatureCollectionQueryHandler,
        IQueryHandler<GetRidesTrackQuery, Result<TrackViewModel>> getRidesTrackQueryHandler,
        IQueryHandler<GetRidesTrackFeatureCollectionQuery, Result<FeatureCollection>> getRidesTrackFeatureCollectionHandler,
        IQueryHandler<GetRidesQuery, Result<RidesViewModel>> getRidesQueryHandler)
    {
        _getVehicleQueryHandler = getVehicleQueryHandler;
        _getVehiclesListQueryHandler = getVehiclesListQueryHandler;

        _createVehicleHandler = createVehicleHandler;
        _updateVehicleHandler = updateVehicleHandler;
        _deleteVehicleHandler = deleteVehicleHandler;

        _getTrackQueryHandler = getTrackQueryHandler;
        _getTrackFeatureCollectionQueryHandler = getTrackFeatureCollectionQueryHandler;

        _getRidesTrackQueryHandler = getRidesTrackQueryHandler;
        _getRidesTrackFeatureCollectionHandler = getRidesTrackFeatureCollectionHandler;

        _getRidesQueryHandler = getRidesQueryHandler;
    }

    // GET: api/Vehicles
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedVehicles))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedVehicles>> GetVehicles([FromQuery] GetVehiclesRequest request)
    {
        Guid managerId = GetCurrentManagerId();

        GetVehiclesListQuery query = new GetVehiclesListQuery
        {
            RequestingManagerId = managerId,
            Limit = request.Limit,
            Offset = request.Offset
        };

        Result<PaginatedVehicles> getVehiclesList = await _getVehiclesListQueryHandler.Handle(query);

        if (getVehiclesList.IsSuccess)
        {
            return Ok(getVehiclesList.Value);
        }

        if (getVehiclesList.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    // GET: api/Vehicles/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VehicleDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VehicleDto>> GetVehicle(Guid id)
    {
        Guid managerId = GetCurrentManagerId();

        GetVehicleQuery query = new GetVehicleQuery
        {
            RequestingManagerId = managerId,
            VehicleId = id
        };

        Result<VehicleDto> getVehicle = await _getVehicleQueryHandler.Handle(query);

        if (getVehicle.IsSuccess)
        {
            return Ok(getVehicle.Value);
        }

        if (getVehicle.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (getVehicle.HasError(e => e.Message == VehiclesHandlersErrors.ManagerNotAllowedToVehicle))
        {
            return Forbid();
        }
        else if (getVehicle.HasError(e => e.Message == VehiclesHandlersErrors.VehicleNotExist))
        {
            return NotFound();
        }
        else
        {
            return BadRequest();
        }
    }

    // PUT: api/Vehicles/5
    [HttpPut("{id}")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutVehicle(Guid id, CreateUpdateVehicleRequest request)
    {
        Guid managerId = GetCurrentManagerId();

        UpdateVehicleCommand command = new UpdateVehicleCommand
        {
            RequestingManagerId = managerId,
            VehicleId = id,
            ModelId = request.ModelId,
            EnterpriseId = request.EnterpriseId,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            DriverIds = request.DriversAssignments.DriversIds,
            ActiveDriverId = request.DriversAssignments.ActiveDriverId,
            AddedToEnterpriseAt = request.AddedToEnterpriseAt
        };

        Result<Guid> updateVehicle = await _updateVehicleHandler.Handle(command);

        if (updateVehicle.IsSuccess)
        {
            return NoContent();
        }

        if (updateVehicle.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        } 
        else if (updateVehicle.HasError(e => e.Message == VehiclesHandlersErrors.VehicleNotExist))
        {
            return NotFound();
        }
        else if (updateVehicle.HasError(e => e.Message == VehiclesHandlersErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else if (updateVehicle.HasError(e => VehiclesErrors.GetErrors().Contains(e.Message)))
        {
            return BadRequest();
        }
        else
        {
            return BadRequest();
        }
    }

    // POST: api/Vehicles
    [HttpPost]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PostVehicle(CreateUpdateVehicleRequest request)
    {
        Guid managerId = GetCurrentManagerId();

        CreateVehicleCommand command = new CreateVehicleCommand
        {
            ModelId = request.ModelId,
            EnterpriseId = request.EnterpriseId,
            RequestingManagerId = managerId,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            DriverIds = request.DriversAssignments.DriversIds,
            ActiveDriverId = request.DriversAssignments.ActiveDriverId,
            AddedToEnterpriseAt = request.AddedToEnterpriseAt
        };

        Result<Guid> createVehicle = await _createVehicleHandler.Handle(command);

        // Success flow
        if (createVehicle.IsSuccess)
        {
            return CreatedAtAction("GetVehicle", new { id = createVehicle.Value }, null);
        }

        // Errors handling
        if (createVehicle.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (createVehicle.HasError(e => e.Message == VehiclesHandlersErrors.VehicleNotExist))
        {
            return NotFound();
        }
        else if (createVehicle.HasError(e => e.Message == VehiclesHandlersErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else if (createVehicle.HasError(e => VehiclesErrors.GetErrors().Contains(e.Message)))
        {
            return BadRequest();
        }
        else
        {
            return BadRequest();
        }
    }

    // DELETE: api/Vehicles/5
    [HttpDelete("{id}")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        Guid managerId = GetCurrentManagerId();

        DeleteVehicleCommand command = new DeleteVehicleCommand 
        {
            RequestingManagerId = managerId,
            VehicleId = id
        };
        
        Result deleteVehicle = await _deleteVehicleHandler.Handle(command);

        // Success flow
        if (deleteVehicle.IsSuccess)
        {
            return NoContent();
        }

        // Errors handling
        if (deleteVehicle.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (deleteVehicle.HasError(e => e.Message == VehiclesHandlersErrors.VehicleNotExist))
        {
            return NotFound();
        }
        else if (deleteVehicle.HasError(e => e.Message == VehiclesHandlersErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else if (deleteVehicle.HasError(e => e.Message == VehiclesHandlersErrors.ForbidDeleteVehicleWithAssignedDrivers))
        {
            return Conflict();
        }
        else if (deleteVehicle.HasError(e => VehiclesErrors.GetErrors().Contains(e.Message)))
        {
            return BadRequest();
        }
        else
        {
            return BadRequest();
        }
    }

    public class GetVehiclesRequest : IPaginationRequest
    {
        [Required]
        [Range(1, 1000)]
        public required uint Limit { get; init; }

        [Required]
        [Range(0, int.MaxValue)]
        public required uint Offset { get; init; }
    }

    public class CreateUpdateVehicleRequest
    {
        [Required]
        public required Guid ModelId { get; set; }

        [Required]
        public required Guid EnterpriseId { get; set; }

        [Required]
        public required string VinNumber { get; set; }

        [Required]
        public required decimal Price { get; set; }

        [Required]
        public required int ManufactureYear { get; set; }

        [Required]
        public required int Mileage { get; set; }

        [Required]
        public required string Color { get; set; }

        [Required]
        public required DriversAssignmentsViewModel DriversAssignments { get; set; }

        [Required]
        public required DateTimeOffset AddedToEnterpriseAt { get; set; }

        public class DriversAssignmentsViewModel
        {
            [Required]
            public required List<Guid> DriversIds { get; set; }

            [Required]
            public required Guid? ActiveDriverId { get; set; }
        }
    }

    #region Geo track

    [HttpGet("{vehicleId}/track")]
    [Produces("application/json", "application/geo+json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrackIndexViewModel))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeatureCollection))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetTrack(Guid vehicleId, [FromQuery] GetTrackRequest request)
    {
        Guid managerId = GetCurrentManagerId();

        Result errorResult;

        if (CheckIsGeoJsonRequested())
        {
            GetTrackFeatureCollectionQuery query = new GetTrackFeatureCollectionQuery
            {
                RequestingManagerId = managerId,
                VehicleId = vehicleId,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            Result<FeatureCollection> getTrack = await _getTrackFeatureCollectionQueryHandler.Handle(query);

            if (getTrack.IsSuccess)
            {
                return Ok(getTrack.Value);
            }

            errorResult = getTrack.ToResult();
        }
        else
        {
            GetTrackQuery query = new GetTrackQuery
            {
                RequestingManagerId = managerId,
                VehicleId = vehicleId,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            Result<TrackViewModel> getTrack = await _getTrackQueryHandler.Handle(query);

            if (getTrack.IsSuccess)
            {
                return Ok(getTrack.Value);
            }

            errorResult = getTrack.ToResult();
        }

        if (errorResult.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (errorResult.HasError(e => e.Message == TrackHandlersErrors.VehicleNotFound))
        {
            return NotFound();
        }
        else if (errorResult.HasError(e => e.Message == TrackHandlersErrors.ManagerNotAllowedToVehicle))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    public class GetTrackRequest
    {
        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public DateTimeOffset EndTime { get; set; }
    }

    public class GeoTimePoint
    {
        public required DateTimeOffset Time { get; set; }

        public double X => Point.X;

        public double Y => Point.Y;

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Point Point { get; set; } = default!;
    }

    #endregion

    #region Rides

    [HttpGet("{vehicleId}/rides/track")]
    [Produces("application/json", "application/geo+json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetRidesResponse))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeatureCollection))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetRidesTrack(GetRidesTrackRequest request)
    {
        Guid managerId = GetCurrentManagerId();

        Result errorResult;

        if (CheckIsGeoJsonRequested())
        {
            GetRidesTrackFeatureCollectionQuery query = new GetRidesTrackFeatureCollectionQuery
            {
                RequestingManagerId = managerId,
                VehicleId = request.VehicleId,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            Result<FeatureCollection> getRidesTrack = await _getRidesTrackFeatureCollectionHandler.Handle(query);

            if (getRidesTrack.IsSuccess)
            {
                return Ok(getRidesTrack.Value);
            }

            errorResult = getRidesTrack.ToResult();
        }
        else
        {
            GetRidesTrackQuery query = new GetRidesTrackQuery
            {
                RequestingManagerId = managerId,
                VehicleId = request.VehicleId,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            Result<TrackViewModel> getRidesTrack = await _getRidesTrackQueryHandler.Handle(query);

            if (getRidesTrack.IsSuccess)
            {
                return Ok(getRidesTrack.Value);
            }

            errorResult = getRidesTrack.ToResult();
        }

        if (errorResult.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (errorResult.HasError(e => e.Message == TrackHandlersErrors.VehicleNotFound))
        {
            return NotFound();
        }
        else if (errorResult.HasError(e => e.Message == TrackHandlersErrors.ManagerNotAllowedToVehicle))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    public class GetRidesTrackRequest
    {
        [FromRoute(Name = "vehicleId")]
        public Guid VehicleId { get; set; }

        [FromQuery]
        public DateTimeOffset StartTime { get; set; }

        [FromQuery]
        public DateTimeOffset EndTime { get; set; }
    }

    public class GetRidesResponse
    {
        public required List<GeoTimePoint> GeoTimePoints { get; set; }
    }


    [HttpGet("{vehicleId}/rides")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RidesViewModel))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetRides(GetRidesRequest request)
    {
        Guid managerId = GetCurrentManagerId();

        GetRidesQuery query = new GetRidesQuery
        {
            RequestingManagerId = managerId,
            VehicleId = request.VehicleId,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        Result<RidesViewModel> getRides = await _getRidesQueryHandler.Handle(query);

        if (getRides.IsSuccess)
        {
            return Ok(getRides.Value);
        }

        if (getRides.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (getRides.HasError(e => e.Message == RidesHandlerErrors.VehicleNotFound))
        {
            return NotFound();
        }
        else if (getRides.HasError(e => e.Message == RidesHandlerErrors.ManagerNotAllowedToVehicle))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    public class GetRidesRequest
    {
        [FromRoute(Name = "vehicleId")]
        public Guid VehicleId { get; set; }

        [FromQuery]
        public DateTimeOffset StartTime { get; set; }

        [FromQuery]
        public DateTimeOffset EndTime { get; set; }
    }
    #endregion

    private bool CheckIsGeoJsonRequested()
    {
        string acceptHeader = Request.Headers.Accept.ToString();
        bool isGeoJsonRequested = acceptHeader.Contains("application/geo+json");
        return isGeoJsonRequested;
    }
}