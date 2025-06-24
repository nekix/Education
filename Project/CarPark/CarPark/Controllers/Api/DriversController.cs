using CarPark.Data;
using CarPark.ViewModels.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleAssignmentViewModel = CarPark.ViewModels.Api.DriverViewModel.VehicleAssignmentViewModel;

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
        return await _context.Drivers
            .GroupJoin(_context.DriverVehicleAssignments,
                d => d.Id,
                dva => dva.DriverId,
                (d, dva) => new DriverViewModel()
                {
                    Id = d.Id,
                    DriverLicenseNumber = d.DriverLicenseNumber,
                    EnterpriseId = d.EnterpriseId,
                    FullName = d.FullName,
                    VehiclesAssignments = dva.Select(a => new VehicleAssignmentViewModel()
                    {
                        VehicleId = a.VehicleId,
                        IsActive = a.IsActiveAssignment
                    }).ToList()
                })
            .ToListAsync();
    }

    // GET: api/Drivers/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<DriverViewModel>> GetDriver(int id)
    {
        DriverViewModel? driverVm = await _context.Drivers
            .GroupJoin(_context.DriverVehicleAssignments,
                d => d.Id,
                dva => dva.DriverId,
                (d, dva) => new DriverViewModel()
                {
                    Id = d.Id,
                    DriverLicenseNumber = d.DriverLicenseNumber,
                    EnterpriseId = d.EnterpriseId,
                    FullName = d.FullName,
                    VehiclesAssignments = dva.Select(a => new VehicleAssignmentViewModel()
                    {
                        VehicleId = a.VehicleId,
                        IsActive = a.IsActiveAssignment
                    }).ToList()
                })
            .FirstOrDefaultAsync(vm => vm.Id == id);

        if (driverVm == null)
        {
            return NotFound();
        }

        return driverVm;
    }
}