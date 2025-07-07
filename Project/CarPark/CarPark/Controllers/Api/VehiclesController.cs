using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.ViewModels.Api;
using DriversAssignmentsViewModel = CarPark.ViewModels.Api.VehicleViewModel.DriversAssignmentsViewModel;
using Microsoft.AspNetCore.Authorization;
using CarPark.Models;
using CarPark.Identity;

namespace CarPark.Controllers.Api;

[Authorize(AppIdentityConst.ManagerPolicy)]
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
        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        return await viewModelQuery.OrderBy(x => x.Id).ToListAsync();
    }

    // GET: api/VehiclesApi/5
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
                DriversAssignments = new DriversAssignmentsViewModel
                {
                    DriversIds = assignments,
                    ActiveDriverId = v.ActiveAssignedDriver.Id
                }
            };

        return vehiclesQuery;
    }
}