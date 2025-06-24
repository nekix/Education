using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.Models;
using CarPark.ViewModels.Api;
using RelatedEntitiesViewModel = CarPark.ViewModels.Api.EnterpriseOverviewViewModel.RelatedEntitiesViewModel;

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
    public async Task<ActionResult<IEnumerable<Enterprise>>> GetEnterprises()
    {
        return await _context.Enterprises.ToListAsync();
    }

    // GET: api/Enterprises/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<Enterprise>> GetEnterprise(int id)
    {
        Enterprise? enterprise = await _context.Enterprises.FindAsync(id);

        if (enterprise == null)
        {
            return NotFound();
        }

        return enterprise;
    }

    [HttpGet("{id}/overview")]
    public async Task<ActionResult<List<EnterpriseOverviewViewModel>>> GetTest(int id)
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

        IQueryable<EnterpriseOverviewViewModel> overviewQuery =
            from e in _context.Enterprises
            let drivers = driversQuery
                .Where(x => x.EnterpriseId == e.Id)
                .Select(x => x.Id)
                .ToList()
            let vehicles = vehiclesQuery
                .Where(x => x.EnterpriseId == e.Id)
                .Select(x => x.Id)
                .ToList()
            select new EnterpriseOverviewViewModel
            {
                Id = e.Id,
                Name = e.Name,
                LegalAddress = e.LegalAddress,
                RelatedEntities = new RelatedEntitiesViewModel()
                {
                    DriversIds = drivers,
                    VehiclesIds = vehicles
                }
            };

        return Ok(await overviewQuery.FirstOrDefaultAsync(q => q.Id == id));
    }
}