using CarPark.Data;
using CarPark.Identity;
using CarPark.Models.Drivers;
using CarPark.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehiclesAssignmentsViewModel = CarPark.ViewModels.Api.DriverViewModel.VehiclesAssignmentsViewModel;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class DriversController : ApiBaseController
{
            private readonly ApplicationDbContext _context;

        public DriversController(ApplicationDbContext context)
    {
                    _context = context;
    }

    // GET: api/Drivers
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DriverViewModel>))]
    public async Task<ActionResult<IEnumerable<DriverViewModel>>> GetDrivers()
    {
        int managerId = GetCurrentManagerId();

        IQueryable<Driver> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<DriverViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        return Ok(await viewModelQuery.OrderBy(x => x.Id).ToListAsync());
    }

    // GET: api/Drivers/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DriverViewModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DriverViewModel>> GetDriver(int id)
    {
        int managerId = GetCurrentManagerId();

        IQueryable<Driver> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<DriverViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        DriverViewModel? viewModel = await viewModelQuery
            .SingleOrDefaultAsync(x => x.Id == id);

        if (viewModel == null)
        {
            return NotFound();
        }

        return viewModel;
    }

    private IQueryable<Driver> GetFilteredByManagerQuery(int managerId)
    {
        IQueryable<Driver> filteredQuery =
            from e in _context.Enterprises
            join d in _context.Drivers on e.Id equals d.EnterpriseId
            where e.Managers.Any(m => m.Id == managerId)
            select d;

        return filteredQuery;
    }

    private static IQueryable<DriverViewModel> TransformToViewModelQuery(IQueryable<Driver> query)
    {
        IQueryable<DriverViewModel> viewModelQuery =
            from d in query
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
                    ActiveVehicleId = d.ActiveAssignedVehicle != null ? d.ActiveAssignedVehicle.Id : null,
                }
            };

        return viewModelQuery;
    }
}