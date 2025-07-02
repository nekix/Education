using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.ViewModels.Api;
using DriversAssignmentsViewModel = CarPark.ViewModels.Api.VehicleViewModel.DriversAssignmentsViewModel;

namespace CarPark.Controllers.Api;

public class VehiclesController : ApiBaseController
{
    private readonly ApplicationDbContext _context;

    public VehiclesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/VehiclesApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleViewModel>>> GetVehicles()
    {
        IQueryable<VehicleViewModel> vehiclesQuery = CreateVehiclesQuery();

        return await vehiclesQuery.ToListAsync();
    }

    // GET: api/VehiclesApi/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<VehicleViewModel>> GetVehicle(int id)
    {
        IQueryable<VehicleViewModel> vehiclesQuery = CreateVehiclesQuery();

        VehicleViewModel? vehicle = await vehiclesQuery.SingleOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound();
        }

        return vehicle;
    }

    private IQueryable<VehicleViewModel> CreateVehiclesQuery()
    {
        IQueryable<VehicleViewModel> vehiclesQuery =
            from v in _context.Vehicles
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
                DriversAssignments = new DriversAssignmentsViewModel
                {
                    DriversIds = assignments,
                    ActiveDriverId = v.ActiveAssignedDriver.Id
                }
            };

        return vehiclesQuery;
    }
}