using CarPark.Data;
using CarPark.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CarPark.ViewModels.Enterprises;

namespace CarPark.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class EnterprisesController : BaseController
{
    private readonly ApplicationDbContext _context;

    public EnterprisesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int managerId = GetCurrentManagerId();

        List<EnterpriseViewModel> enterprises = await _context.Enterprises
            .Where(e => e.Managers.Any(m => m.Id == managerId))
            .Select(e => new EnterpriseViewModel { Id = e.Id, Name = e.Name })
            .OrderBy(e => e.Id)
            .ToListAsync();

        return View(enterprises);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        int managerId = GetCurrentManagerId();

        EnterpriseDetailsViewModel? enterprise = await _context
            .Enterprises
            .Where(e => e.Managers.Any(m => m.Id == managerId))
            .Where(e => e.Id == id)
            .Select(e => new EnterpriseDetailsViewModel
            {
                Id = e.Id,
                Name = e.Name,
                LegalAddress = e.LegalAddress,
                Vehicles = _context.Vehicles
                    .Where(v => v.EnterpriseId == e.Id)
                    .Select(v => new EnterpriseDetailsViewModel.VehicleViewModel
                    {
                        Id = v.Id,
                        VinNumber = v.VinNumber
                    })
                    .OrderBy(v => v.Id)
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (enterprise == null)
        {
            return NotFound();
        }

        return View(enterprise);
    }
}