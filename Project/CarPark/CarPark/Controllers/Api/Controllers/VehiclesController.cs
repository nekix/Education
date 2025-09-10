using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using CarPark.Identity;
using CarPark.Attributes;
using CarPark.Models.Vehicles;
using FluentResults;
using CarPark.Shared.CQ;
using CarPark.Controllers.Api.Abstract;
using CarPark.Models.TzInfos;
using CarPark.Services;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class VehiclesController : ApiBaseController
{
    private readonly IVehiclesDbSet _vehiclesSet;
    private readonly IEnterprisesDbSet _enterprisesSet;
    private readonly ICommandHandler<CreateVehicleCommand, Result<int>> _createVehicleHandler;
    private readonly ICommandHandler<UpdateVehicleCommand, Result<int>> _updateVehicleHandler;
    private readonly ICommandHandler<DeleteVehicleCommand, Result> _deleteVehicleHandler;
    private readonly TimeZoneConversionService _timeZoneConversionService;

    public VehiclesController(IVehiclesDbSet vehiclesSet,
        IEnterprisesDbSet enterprisesSet,
        ICommandHandler<CreateVehicleCommand, Result<int>> createVehicleHandler,
        ICommandHandler<UpdateVehicleCommand, Result<int>> updateVehicleHandler,
        ICommandHandler<DeleteVehicleCommand, Result> deleteVehicleHandler,
        TimeZoneConversionService timeZoneConversionService)
    {
        _vehiclesSet = vehiclesSet;
        _enterprisesSet = enterprisesSet;
        _createVehicleHandler = createVehicleHandler;
        _updateVehicleHandler = updateVehicleHandler;
        _deleteVehicleHandler = deleteVehicleHandler;
        _timeZoneConversionService = timeZoneConversionService;
    }

    // GET: api/Vehicles
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VehicleViewModel>))]
    public async Task<ActionResult<GetVehiclesResponse>> GetVehicles([FromQuery] GetVehiclesRequest request)
    {
        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<Vehicle> orderedQuery = originalQuery
            .OrderBy(x => x.EnterpriseId)
            .ThenBy(x => x.Color)
            .ThenBy(x => x.VinNumber);

        uint total = (uint)orderedQuery.Count();

        IQueryable<Vehicle> paginatedQuery = orderedQuery
            .Skip((int)request.Offset)
            .Take((int)request.Limit);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(paginatedQuery);

        List<VehicleViewModel> viewModels = await viewModelQuery
            .OrderBy(x => x.EnterpriseId)
            .ThenBy(x => x.Color)
            .ThenBy(x => x.VinNumber)
            .ToListAsync();

        // Get enterprises for timezone info
        Dictionary<int, TzInfo?> enterpriseTimeZones = await _enterprisesSet.Enterprises
            .Where(e => viewModels.Select(v => v.EnterpriseId).Contains(e.Id))
            .Select(e => new { e.Id, e.TimeZone })
            .ToDictionaryAsync(e => e.Id, e => e.TimeZone);

        // Convert dates to enterprise timezone
        foreach (VehicleViewModel model in viewModels)
        {
            if (enterpriseTimeZones.TryGetValue(model.EnterpriseId, out TzInfo? timeZoneInfo))
            {
                model.AddedToEnterpriseAt = _timeZoneConversionService.ConvertToEnterpriseTimeZone(
                    model.AddedToEnterpriseAt,
                    timeZoneInfo);
            }
        }

        GetVehiclesResponse response = new GetVehiclesResponse
        {
            Data = viewModels,
            Meta = new GetVehiclesResponse.Metadata()
            {
                Limit = request.Limit,
                Offset = request.Offset,
                Total = total
            }
        };

        return Ok(response);
    }

    // GET: api/Vehicles/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VehicleViewModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleViewModel>> GetVehicle(int id)
    {
        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        VehicleViewModel? viewModel = await viewModelQuery
            .SingleOrDefaultAsync(v => v.Id == id);

        if (viewModel == null)
        {
            return NotFound();
        }

        // Get enterprise timezone info
        TzInfo? enterpriseTimeZone = await _enterprisesSet.Enterprises
            .Where(e => e.Id == viewModel.EnterpriseId)
            .Select(e => e.TimeZone)
            .FirstOrDefaultAsync();

        // Convert date to enterprise timezone
        viewModel.AddedToEnterpriseAt = _timeZoneConversionService.ConvertToEnterpriseTimeZone(
            viewModel.AddedToEnterpriseAt,
            enterpriseTimeZone);

        return viewModel;
    }

    // PUT: api/Vehicles/5
    [HttpPut("{id}")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutVehicle(int id, CreateUpdateVehicleRequest request)
    {
        int managerId = GetCurrentManagerId();

        UpdateVehicleCommand command = new UpdateVehicleCommand
        {
            Id = id,
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

        Result<int> result = await _updateVehicleHandler.Handle(command);

        // Success flow
        if (!result.IsFailed)
            return NoContent();

        // Errors handling
        if (result.HasError(e => e.Message == UpdateVehicleCommand.Errors.VehicleNotFound))
        {
            return NotFound();
        }

        // Undefined errors
        return BadRequest();
    }

    // POST: api/Vehicles
    [HttpPost]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> PostVehicle(CreateUpdateVehicleRequest request)
    {
        int managerId = GetCurrentManagerId();

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

        Result<int> result = await _createVehicleHandler.Handle(command);

        // Success flow
        if (result.IsSuccess)
        {
            return CreatedAtAction("GetVehicle", new { id = result.Value }, null);
        }

        // Errors handling
        if (result.HasError(e => e.Message == CreateVehicleCommand.Errors.AccessDenied))
        {
            return Forbid();
        }

        // Undefined errors
        return BadRequest();
    }

    // DELETE: api/Vehicles/5
    [HttpDelete("{id}")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        int managerId = GetCurrentManagerId();

        DeleteVehicleCommand command = new DeleteVehicleCommand 
        { 
            Id = id,
            RequestingManagerId = managerId
        };
        
        Result result = await _deleteVehicleHandler.Handle(command);

        // Success flow
        if (result.IsSuccess) 
            return NoContent();

        // Errors handling
        if (result.HasError(e => e.Message == DeleteVehicleCommand.Errors.NotFound))
        {
            return NotFound();
        }

        if (result.HasError(e => e.Message == DeleteVehicleCommand.Errors.AccessDenied))
        {
            return Forbid();
        }

        if (result.HasError(e => e.Message == DeleteVehicleCommand.Errors.Conflict))
        {
            return Conflict();
        }

        // Undefined errors
        return BadRequest();
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

    public class GetVehiclesResponse : IPaginationModel<VehicleViewModel, GetVehiclesResponse.Metadata>
    {
        public required Metadata Meta { get; init; }

        public required IEnumerable<VehicleViewModel> Data { get; init; }

        public class Metadata : IPaginationMetadata
        {
            public uint Offset { get; init; }

            public uint Limit { get; init; }

            public uint Total { get; init; }
        }
    }

    public class CreateUpdateVehicleRequest
    {
        [Required]
        public required int ModelId { get; set; }

        [Required]
        public required int EnterpriseId { get; set; }

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
            public required List<int> DriversIds { get; set; }

            [Required]
            public required int? ActiveDriverId { get; set; }
        }
    }

    private IQueryable<Vehicle> GetFilteredByManagerQuery(int managerId)
    {
        IQueryable<int> enterpriseIds = _enterprisesSet.Enterprises
            .Where(e => e.Managers.Any(m => m.Id == managerId))
            .Select(e => e.Id);

        return _vehiclesSet.Vehicles
            .Where(v => enterpriseIds.Contains(v.EnterpriseId));
    }

    private IQueryable<VehicleViewModel> TransformToViewModelQuery(IQueryable<Vehicle> query)
    {
        IQueryable<VehicleViewModel> vehiclesQuery =
            from v in query
            join e in _enterprisesSet.Enterprises on v.EnterpriseId equals e.Id
            from d in v.AssignedDrivers.DefaultIfEmpty()
            group d by new
            {
                v.Id,
                v.ModelId,
                v.EnterpriseId,
                v.VinNumber,
                v.Price,
                v.ManufactureYear,
                v.Mileage,
                v.Color,
                v.AddedToEnterpriseAt,
                ActiveDriverId = v.ActiveAssignedDriver != null ? v.ActiveAssignedDriver.Id : (int?)null
            } into g
            select new VehicleViewModel
            {
                Id = g.Key.Id,
                ModelId = g.Key.ModelId,
                EnterpriseId = g.Key.EnterpriseId,
                VinNumber = g.Key.VinNumber,
                Price = g.Key.Price,
                ManufactureYear = g.Key.ManufactureYear,
                Mileage = g.Key.Mileage,
                Color = g.Key.Color,
                AddedToEnterpriseAt = g.Key.AddedToEnterpriseAt,
                DriversAssignments = new VehicleViewModel.DriversAssignmentsViewModel
                {
                DriversIds = EF.Functions.ArrayAgg(
                    g.Where(x => x != null)
                        .Select(x => x.Id)
                        .Order()),
                    ActiveDriverId = g.Key.ActiveDriverId
                }
            };

        return vehiclesQuery;
    }


    #region Geo track

    public ActionResult<GetTrackResponse> GetTrack(GetTrackRequest request)
    {
        return Ok();
    }

    public class GetTrackRequest
    {
        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public DateTimeOffset EndTime { get; set; }
    }

    public class GetTrackResponse
    {
        
    }

    #endregion
}