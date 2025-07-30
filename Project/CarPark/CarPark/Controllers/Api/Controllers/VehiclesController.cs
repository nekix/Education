using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using CarPark.Identity;
using Microsoft.Build.Framework;
using CarPark.Attributes;
using CarPark.Models.Vehicles;
using FluentResults;
using CarPark.Shared.CQ;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class VehiclesController : ApiBaseController
{
    private readonly IVehiclesDbSet _set;
    private readonly IEnterprisesDbSet _enterprisesSet;
    private readonly ICommandHandler<CreateVehicleCommand, Result<int>> _createVehicleHandler;
    private readonly ICommandHandler<UpdateVehicleCommand, Result<int>> _updateVehicleHandler;
    private readonly ICommandHandler<DeleteVehicleCommand, Result> _deleteVehicleHandler;

    public VehiclesController(IVehiclesDbSet set,
        IEnterprisesDbSet enterprisesSet,
        ICommandHandler<CreateVehicleCommand, Result<int>> createVehicleHandler,
        ICommandHandler<UpdateVehicleCommand, Result<int>> updateVehicleHandler,
        ICommandHandler<DeleteVehicleCommand, Result> deleteVehicleHandler)
    {
        _set = set;
        _enterprisesSet = enterprisesSet;
        _createVehicleHandler = createVehicleHandler;
        _updateVehicleHandler = updateVehicleHandler;
        _deleteVehicleHandler = deleteVehicleHandler;
    }

    // GET: api/Vehicles
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VehicleViewModel>))]
    public async Task<ActionResult<IEnumerable<VehicleViewModel>>> GetVehicles()
    {
        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        return Ok(await viewModelQuery.OrderBy(x => x.Id).ToListAsync());
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

        return viewModel;
    }

    // PUT: api/Vehicles/5
    [HttpPut("{id}")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutVehicle(int id, CreateUpdateVehicleRequest request)
    {
        UpdateVehicleCommand command = new UpdateVehicleCommand
        {
            Id = id,
            ModelId = request.ModelId,
            EnterpriseId = request.EnterpriseId,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            DriverIds = request.DriversAssignments.DriversIds,
            ActiveDriverId = request.DriversAssignments.ActiveDriverId
        };

        Result<int> result = await _updateVehicleHandler.Handle(command);

        // Success flow
        if (!result.IsFailed)
            return NoContent();

        // Errors handling
        if (result.HasError(e => e.Message == UpdateVehicleCommand.Errors.NotFound))
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
    public async Task<ActionResult> PostVehicle(CreateUpdateVehicleRequest request)
    {
        CreateVehicleCommand command = new CreateVehicleCommand
        {
            ModelId = request.ModelId,
            EnterpriseId = request.EnterpriseId,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            DriverIds = request.DriversAssignments.DriversIds,
            ActiveDriverId = request.DriversAssignments.ActiveDriverId
        };

        Result<int> result = await _createVehicleHandler.Handle(command);

        // Success flow
        if (result.IsSuccess)
        {
            return CreatedAtAction("GetVehicle", new { id = result.Value }, null);
        }

        // Undefined errors
        return BadRequest();
    }

    // DELETE: api/Vehicles/5
    [HttpDelete("{id}")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        DeleteVehicleCommand command = new DeleteVehicleCommand { Id = id };
        
        Result result = await _deleteVehicleHandler.Handle(command);

        // Success flow
        if (!result.IsFailed) 
            return NoContent();

        // Errors handling
        if (result.HasError(e => e.Message == DeleteVehicleCommand.Errors.NotFound))
        {
            return NotFound();
        }

        if (result.HasError(e => e.Message == DeleteVehicleCommand.Errors.Conflict))
        {
            return Conflict();
        }

        // Undefined errors
        return BadRequest();
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
        IQueryable<Vehicle> filteredQuery =
            from e in _enterprisesSet.Enterprises
            join v in _set.Vehicles on e.Id equals v.EnterpriseId
            where e.Managers.Any(m => m.Id == managerId)
            select v;

        return filteredQuery;
    }

    private static IQueryable<VehicleViewModel> TransformToViewModelQuery(IQueryable<Vehicle> query)
    {
        IQueryable<VehicleViewModel> vehiclesQuery =
            from v in query
            let assignments = v.AssignedDrivers
                .Select(ad => ad.Id)
                .Order()
                .ToList()
            select new VehicleViewModel
            {
                Id = v.Id,
                ModelId = v.ModelId,
                EnterpriseId = v.EnterpriseId,
                VinNumber = v.VinNumber,
                Price = v.Price,
                ManufactureYear = v.ManufactureYear,
                Mileage = v.Mileage,
                Color = v.Color,
                DriversAssignments = new VehicleViewModel.DriversAssignmentsViewModel
                {
                    DriversIds = assignments,
                    ActiveDriverId = v.ActiveAssignedDriver != null ? v.ActiveAssignedDriver.Id : null,
                }
            };

        return vehiclesQuery;
    }
}