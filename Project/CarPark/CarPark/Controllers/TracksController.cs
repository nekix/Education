using CarPark.Data;
using CarPark.Enterprises;
using CarPark.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
[Route("enterprises/{enterpriseId}/vehicles/{vehicleId}/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class TracksController : BaseController
{
    private readonly ApplicationDbContext _context;

    public TracksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<ActionResult> Index(
        [FromRoute] int enterpriseId, 
        [FromRoute] int vehicleId,
        [FromQuery] DateTimeOffset startDate,
        [FromQuery] DateTimeOffset endDate)
    {
        int managerId = GetCurrentManagerId();

        Enterprise? enterprise = await _context.Enterprises
            .Include(e => e.TimeZone)
            .Include(e => e.Managers)
            .FirstOrDefaultAsync(e => e.Id == enterpriseId);

        if (enterprise == null)
        {
            return NotFound();
        }

        if (enterprise.Managers.All(m => m.Id != managerId))
        {
            return Forbid();
        }

        TrackIndexViewModel vm = new TrackIndexViewModel
        {
            VehicleId = vehicleId,
            StartDate = startDate,
            EndDate = endDate
        };

        return View(vm);
    }
}

public class TrackIndexViewModel
{
    public required int VehicleId { get; init; }

    public required DateTimeOffset StartDate { get; init; }

    public required DateTimeOffset EndDate { get; init; }
}