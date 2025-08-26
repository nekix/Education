using CarPark.Data;
using CarPark.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CarPark.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CarPark.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class EnterprisesController : Controller
{
    private readonly ApplicationDbContext _context;

    public EnterprisesController(ApplicationDbContext context)
    {
        _context = context;
    }

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

    private int GetCurrentManagerId()
    {
        string? managerIdText = User.FindFirstValue(AppIdentityConst.ManagerIdClaim);

        return int.Parse(managerIdText!);
    }
}