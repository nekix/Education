using CarPark.Data;
using CarPark.ViewModels.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehiclesAssignmentsViewModel = CarPark.ViewModels.Api.DriverViewModel.VehiclesAssignmentsViewModel;

namespace CarPark.Controllers.Api;

public class DriversController : ApiBaseController
{
    private readonly ApplicationDbContext _context;

    public DriversController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Drivers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DriverViewModel>>> GetDrivers()
    {
        IQueryable<DriverViewModel> vehiclesQuery = CreateDriversQuery();

        return await vehiclesQuery.ToListAsync();
    }

    // GET: api/Drivers/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<DriverViewModel>> GetDriver(int id)
    {
        IQueryable<DriverViewModel> driversQuery = CreateDriversQuery();

        DriverViewModel? driver = await driversQuery.SingleOrDefaultAsync(v => v.Id == id);

        if (driver == null)
        {
            return NotFound();
        }

        return driver;
    }

    private IQueryable<DriverViewModel> CreateDriversQuery()
    {
        IQueryable<DriverViewModel> vehiclesQuery =
            from d in _context.Drivers
            let assignments = d.AssignedVehicles
                .Select(av => av.Id)
                .Order()
                .ToList()
            select new DriverViewModel
            {
                Id = d.Id,
                EnterpriseId = d.EnterpriseId,
                FullName = d.FullName,
                DriverLicenseNumber = d.DriverLicenseNumber,
                VehiclesAssignments = new VehiclesAssignmentsViewModel
                {
                    VehiclesIds = assignments,
                    ActiveVehicleId = d.ActiveAssignedVehicle.Id
                }
            };

        return vehiclesQuery;
    }
}