using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.Models;
using CarPark.ViewModels.Api;
using RelatedEntitiesViewModel = CarPark.ViewModels.Api.EnterpriseViewModel.RelatedEntitiesViewModel;

namespace CarPark.Controllers.Api;

public class EnterprisesController : ApiBaseController
{
    private readonly ApplicationDbContext _context;

    public EnterprisesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Enterprises
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnterpriseViewModel>>> GetEnterprises()
    {
        IQueryable<EnterpriseViewModel> enterprisesQuery = CreateEnterprisesQuery();

        return await enterprisesQuery.ToListAsync();
    }

    // GET: api/Enterprises/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<EnterpriseViewModel>> GetEnterprise(int id)
    {
        IQueryable<EnterpriseViewModel> enterprisesQuery = CreateEnterprisesQuery();

        EnterpriseViewModel? enterprise = await enterprisesQuery.SingleOrDefaultAsync(e => e.Id == id);

        if (enterprise == null)
        {
            return NotFound();
        }

        return enterprise;
    }

    private IQueryable<EnterpriseViewModel> CreateEnterprisesQuery()
    {
        var driversQuery =
            from d in _context.Drivers
            select new
            {
                d.EnterpriseId,
                d.Id
            };

        var vehiclesQuery =
            from v in _context.Vehicles
            select new
            {
                v.EnterpriseId,
                v.Id
            };

        IQueryable<EnterpriseViewModel> enterprisesQuery =
            from e in _context.Enterprises
            let drivers = driversQuery
                .Where(d => d.EnterpriseId == e.Id)
                .Select(d => d.Id)
                .OrderBy(id => id)
                .ToList()
            let vehicles = vehiclesQuery
                .Where(v => v.EnterpriseId == e.Id)
                .Select(v => v.Id)
                .OrderBy(id => id)
                .ToList()
            select new EnterpriseViewModel
            {
                Id = e.Id,
                Name = e.Name,
                LegalAddress = e.LegalAddress,
                RelatedEntities = new RelatedEntitiesViewModel
                {
                    DriversIds = drivers,
                    VehiclesIds = vehicles
                }
            };

        return enterprisesQuery.OrderBy(e => e.Id);
    }
}