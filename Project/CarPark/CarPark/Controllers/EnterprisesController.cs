using CarPark.Data;
using CarPark.Enterprises;
using CarPark.Identity;
using CarPark.Managers;
using CarPark.Services.TimeZones;
using CarPark.TimeZones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CarPark.ViewModels.Enterprises;

namespace CarPark.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class EnterprisesController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly LocalIcuTimezoneService _timezoneService; 

    public EnterprisesController(ApplicationDbContext context,
        LocalIcuTimezoneService timezoneService)
    {
        _context = context;
        _timezoneService = timezoneService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        Guid managerId = GetCurrentManagerId();

        List<EnterpriseViewModel> enterprises = await _context.Enterprises
            .Where(e => e.Managers.Any(m => m.Id == managerId))
            .Select(e => new EnterpriseViewModel
            {
                Id = e.Id,
                Name = e.Name,
                LegalAddress = e.LegalAddress
            })
            .OrderBy(e => e.Id)
            .ToListAsync();

        return View(enterprises);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Guid managerId = GetCurrentManagerId();

        var enterprise = await _context
            .Enterprises
            .Where(e => e.Managers.Any(m => m.Id == managerId))
            .Where(e => e.Id == id)
            .Select(e => new
            {
                Id = e.Id,
                Name = e.Name,
                LegalAddress = e.LegalAddress,
                TimeZone = e.TimeZone
            })
            .FirstOrDefaultAsync();


        if (enterprise == null)
        {
            return NotFound();
        }

        EnterpriseDetailsViewModel enterpriseVm = new EnterpriseDetailsViewModel
        {
            Id = enterprise.Id,
            Name = enterprise.Name,
            LegalAddress = enterprise.LegalAddress,
            TimeZone = enterprise.TimeZone != null
                ? new EnterpriseDetailsViewModel.TimeZoneViewModel
                {
                    Id = enterprise.TimeZone.Id,
                    Name = GetUserFriendlyTimeZoneName(enterprise.TimeZone.IanaTzId, enterprise.TimeZone.WindowsTzId,
                        "ru-RU")
                }
                : null
        };

        return View(enterpriseVm);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        EnterpriseCreateEditViewModel viewModel = new EnterpriseCreateEditViewModel
        {
            Name = string.Empty,
            LegalAddress = string.Empty,
            TimeZoneId = null,
            AvailableTimeZones = await GetAvailableTimeZonesSelectList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EnterpriseCreateEditViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            Guid managerId = GetCurrentManagerId();
            Manager? manager = await _context.Managers.FindAsync(managerId);

            if (manager == null)
            {
                return BadRequest("Manager not found");
            }

            TzInfo? timeZone = null;
            if (viewModel.TimeZoneId.HasValue)
            {
                timeZone = await _context.TzInfos.FindAsync(viewModel.TimeZoneId.Value);
            }

            Enterprise enterprise = new Enterprise
            {
                Id = default,
                Name = viewModel.Name,
                LegalAddress = viewModel.LegalAddress,
                TimeZone = timeZone,
                Managers = new List<Manager> { manager }
            };

            _context.Enterprises.Add(enterprise);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        viewModel.AvailableTimeZones = await GetAvailableTimeZonesSelectList();
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Guid managerId = GetCurrentManagerId();

        Enterprise? enterprise = await _context.Enterprises
            .Include(e => e.TimeZone)
            .Where(e => e.Managers.Any(m => m.Id == managerId))
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enterprise == null)
        {
            return NotFound();
        }

        EnterpriseCreateEditViewModel viewModel = new EnterpriseCreateEditViewModel
        {
            Id = enterprise.Id,
            Name = enterprise.Name,
            LegalAddress = enterprise.LegalAddress,
            TimeZoneId = enterprise.TimeZone?.Id,
            AvailableTimeZones = await GetAvailableTimeZonesSelectList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EnterpriseCreateEditViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            Guid managerId = GetCurrentManagerId();

            Enterprise? enterprise = await _context.Enterprises
                .Include(e => e.Managers)
                .Include(e => e.TimeZone)
                .Where(e => e.Managers.Any(m => m.Id == managerId))
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enterprise == null)
            {
                return NotFound();
            }

            TzInfo? timeZone = null;
            if (viewModel.TimeZoneId.HasValue)
            {
                timeZone = await _context.TzInfos.FindAsync(viewModel.TimeZoneId.Value);
            }

            enterprise.Name = viewModel.Name;
            enterprise.LegalAddress = viewModel.LegalAddress;
            enterprise.TimeZone = timeZone;

            try
            {
                _context.Update(enterprise);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EnterpriseExists(enterprise.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        viewModel.AvailableTimeZones = await GetAvailableTimeZonesSelectList();
        return View(viewModel);
    }

    private async Task<bool> EnterpriseExists(Guid id)
    {
        return await _context.Enterprises.AnyAsync(e => e.Id == id);
    }

    private async Task<List<SelectListItem>> GetAvailableTimeZonesSelectList()
    {
        List<TzInfo> timeZones = await _context.TzInfos
            .OrderBy(tz => tz.IanaTzId)
            .ToListAsync();

        List<SelectListItem> result = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "-- Select Time Zone --" }
        };

        foreach (TzInfo timeZone in timeZones)
        {
            string displayName = GetUserFriendlyTimeZoneName(timeZone.IanaTzId, timeZone.WindowsTzId, "ru-RU");
            result.Add(new SelectListItem
            {
                Value = timeZone.Id.ToString(),
                Text = displayName
            });
        }

        return result.OrderBy(x => x.Text).ToList();
    }

    private string GetUserFriendlyTimeZoneName(string timezoneIanaId, string locale)
    {
        string? timeZoneDisplayName = _timezoneService.GetTimeZoneDisplayName(timezoneIanaId, DisplayNameType.Standard, locale);
        if (timeZoneDisplayName == null)
            throw new Exception($"Не удалость получить DisplayName для timezone = '{timezoneIanaId}'");

        TimeZoneInfo tzInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneIanaId);

        return $"UTC-{tzInfo.BaseUtcOffset:hh\\:mm} {timeZoneDisplayName} - {timezoneIanaId}";
    }

    private string GetUserFriendlyTimeZoneName(string timezoneIanaId, string windowsTimeZoneId, string locale)
    {
        string? timeZoneDisplayName = _timezoneService.GetTimeZoneDisplayName(timezoneIanaId, DisplayNameType.Standard, locale);
        if (timeZoneDisplayName == null)
            throw new Exception($"Не удалость получить DisplayName для timezone = '{timezoneIanaId}'");

        TimeZoneInfo tzInfo = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);

        return $"UTC-{tzInfo.BaseUtcOffset:hh\\:mm} {timeZoneDisplayName} - {timezoneIanaId}";
    }
}