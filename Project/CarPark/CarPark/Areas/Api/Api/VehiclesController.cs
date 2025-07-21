using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using CarPark.Models;
using CarPark.Identity;
using Microsoft.Build.Framework;
using CarPark.Attributes;

namespace CarPark.Areas.Api.Api;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class VehiclesController : ApiBaseController
{
    private readonly ApplicationDbContext _context;

    public VehiclesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Vehicles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleViewModel>>> GetVehicles()
    {
        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        return await viewModelQuery.OrderBy(x => x.Id).ToListAsync();
    }

    // GET: api/Vehicles/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
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
    [ProducesDefaultResponseType]
    public async Task<IActionResult> PutVehicle(int id, CreateUpdateVehicleRequest request)
    {
        Vehicle? vehicle = await _context.Vehicles
            .Include(d => d.AssignedDrivers)
            .Include(d => d.ActiveAssignedDriver)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound();
        }

        if (request.DriversAssignments.DriversIds.Any())
        {
            List<Driver> drivers = await _context.Drivers
                .Where(d => d.EnterpriseId == request.EnterpriseId)
                .Where(d => request.DriversAssignments.DriversIds.Contains(d.Id))
                .ToListAsync();

            if (drivers.Count != request.DriversAssignments.DriversIds.Count)
            {
                return BadRequest();
            }

            vehicle.AssignedDrivers = drivers;

            if (request.DriversAssignments.ActiveDriverId != null)
            {
                int activeDriverId = request.DriversAssignments.ActiveDriverId.Value;

                if (!request.DriversAssignments.DriversIds.Contains(activeDriverId))
                {
                    return BadRequest();
                }

                vehicle.ActiveAssignedDriver = drivers.First(d => d.Id == request.DriversAssignments.ActiveDriverId);
            }
        }

        vehicle.ModelId = request.ModelId;
        vehicle.EnterpriseId = request.EnterpriseId;
        vehicle.VinNumber = request.VinNumber;
        vehicle.Price = request.Price;
        vehicle.ManufactureYear = request.ManufactureYear;
        vehicle.Mileage = request.Mileage;
        vehicle.Color = request.Color;

        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Vehicles
    [HttpPost]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> PostVehicle(CreateUpdateVehicleRequest request)
    {
        Vehicle vehicle = new Vehicle
        {
            ModelId = request.ModelId,
            EnterpriseId = request.EnterpriseId,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
        };

        if (request.DriversAssignments.DriversIds.Any())
        {
            List<Driver> drivers = await _context.Drivers
                .Where(d => d.EnterpriseId == request.EnterpriseId)
                .Where(d => request.DriversAssignments.DriversIds.Contains(d.Id))
                .ToListAsync();

            if (drivers.Count != request.DriversAssignments.DriversIds.Count)
            {
                return BadRequest();
            }

            vehicle.AssignedDrivers = drivers;

            if (request.DriversAssignments.ActiveDriverId != null)
            {
                int activeDriverId = request.DriversAssignments.ActiveDriverId.Value;

                if (!request.DriversAssignments.DriversIds.Contains(activeDriverId))
                {
                    return BadRequest();
                }

                vehicle.ActiveAssignedDriver = drivers.First(d => d.Id == request.DriversAssignments.ActiveDriverId);
            }
        }

        _context.Vehicles.Add(vehicle);

        await _context.SaveChangesAsync();

        VehicleViewModel viewModel = MapToViewModel(vehicle);

        return CreatedAtAction("GetVehicle", new { id = vehicle.Id }, viewModel);
    }

    // DELETE: api/Vehicles/5
    [HttpDelete("{id}")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        Vehicle? vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();

        return NoContent();
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

    private VehicleViewModel MapToViewModel(Vehicle vehicle)
    {
        return new VehicleViewModel
        {
            Id = vehicle.Id,
            ModelId = vehicle.ModelId,
            EnterpriseId = vehicle.EnterpriseId,
            VinNumber = vehicle.VinNumber,
            Price = vehicle.Price,
            ManufactureYear = vehicle.ManufactureYear,
            Mileage = vehicle.Mileage,
            Color = vehicle.Color,
            DriversAssignments = new VehicleViewModel.DriversAssignmentsViewModel
            {
                DriversIds = vehicle.AssignedDrivers.Select(x => x.Id),
                ActiveDriverId = vehicle.ActiveAssignedDriver?.Id
            }
        };
    }

    private IQueryable<Vehicle> GetFilteredByManagerQuery(int managerId)
    {
        IQueryable<Vehicle> filteredQuery =
            from e in _context.Enterprises
            join v in _context.Vehicles on e.Id equals v.EnterpriseId
            where e.Managers.Any(m => m.Id == managerId)
            select v;

        return filteredQuery;
    }

    private IQueryable<VehicleViewModel> TransformToViewModelQuery(IQueryable<Vehicle> query)
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
                    ActiveDriverId = v.ActiveAssignedDriver.Id
                }
            };

        return vehiclesQuery;
    }
}