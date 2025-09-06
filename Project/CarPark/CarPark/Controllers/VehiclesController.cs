using CarPark.Attributes;
using CarPark.Data;
using CarPark.Identity;
using CarPark.Models.Vehicles;
using CarPark.Shared.CQ;
using CarPark.ViewModels.Vehicles;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleViewModel = CarPark.ViewModels.Vehicles.VehicleViewModel;
using CarPark.Models.Enterprises;
using CarPark.Services;
using CarPark.ViewModels.Common;

namespace CarPark.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
[Route("enterprises/{enterpriseId}/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class VehiclesController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly ICommandHandler<DeleteVehicleCommand, Result> _deleteHandler;
    private readonly ICommandHandler<UpdateVehicleCommand, Result<int>> _updateHandler;
    private readonly ICommandHandler<CreateVehicleCommand, Result<int>> _createHandler;
    private readonly TimeZoneConversionService _timeZoneConversionService;

    public VehiclesController(ApplicationDbContext context,
        ICommandHandler<DeleteVehicleCommand, Result> deleteHandler,
        ICommandHandler<UpdateVehicleCommand, Result<int>> updateHandler,
        ICommandHandler<CreateVehicleCommand, Result<int>> createHandler,
        TimeZoneConversionService timeZoneConversionService)
    {
        _context = context;
        _deleteHandler = deleteHandler;
        _updateHandler = updateHandler;
        _createHandler = createHandler;
        _timeZoneConversionService = timeZoneConversionService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromRoute] int enterpriseId, int? page)
    {
        int managerId = GetCurrentManagerId();
        int pageSize = 10; // Количество записей на страницу
        int pageNumber = page ?? 1;

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerAndEnterpriseQuery(managerId, enterpriseId);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery)
            .OrderBy(v => v.Id);

        PaginatedList<VehicleViewModel> paginatedViewModels = await PaginatedList<VehicleViewModel>.CreateAsync(
            viewModelQuery, pageNumber, pageSize);
        
        // Get enterprise for timezone info
        Enterprise? enterprise = await _context.Enterprises
            .Include(e => e.TimeZone)
            .FirstOrDefaultAsync(e => e.Id == enterpriseId);

        if (enterprise == null)
        {
            return NotFound();
        }

        ViewBag.Enterprise = new { Id = enterprise.Id, Name = enterprise.Name };

        // Convert dates to appropriate timezone
        int? clientTimeZoneOffset = GetClientTimeZoneOffset();
        foreach (VehicleViewModel model in paginatedViewModels)
        {
            model.AddedToEnterpriseAt = _timeZoneConversionService.ConvertForClientDisplay(
                model.AddedToEnterpriseAt,
                enterprise.TimeZone,
                clientTimeZoneOffset);
        }

        return View(paginatedViewModels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details([FromRoute] int enterpriseId, int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerAndEnterpriseQuery(managerId, enterpriseId);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        VehicleViewModel? viewModel = await viewModelQuery
            .SingleOrDefaultAsync(v => v.Id == id);

        if (viewModel == null)
        {
            return NotFound();
        }

        // Get enterprise for timezone info and ViewBag
        Enterprise? enterprise = await _context.Enterprises
            .Include(e => e.TimeZone)
            .FirstOrDefaultAsync(e => e.Id == enterpriseId);

        if (enterprise == null)
        {
            return NotFound();
        }

        ViewBag.Enterprise = new { Id = enterprise.Id, Name = enterprise.Name };

        // Convert date to appropriate timezone
        int? clientTimeZoneOffset = GetClientTimeZoneOffset();
        viewModel.AddedToEnterpriseAt = _timeZoneConversionService.ConvertForClientDisplay(
            viewModel.AddedToEnterpriseAt,
            enterprise.TimeZone,
            clientTimeZoneOffset);

        return View(viewModel);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create([FromRoute] int enterpriseId)
    {
        int managerId = GetCurrentManagerId();

        List<ModelOverview> models = await _context.Models
            .OrderBy(m => m.ModelName)
            .Select(m => new ModelOverview
            {
                Id = m.Id,
                ModelName = m.ModelName
            })
            .ToListAsync();

        ViewBag.Models = models;
        
        Enterprise? enterprise = await _context.Enterprises
            .FirstOrDefaultAsync(e => e.Id == enterpriseId);

        if (enterprise == null)
        {
            return NotFound();
        }

        ViewBag.Enterprise = new { Id = enterprise.Id, Name = enterprise.Name };

        return View();
    }

    [HttpPost("create")]
    [AppValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUpdateVehicleRequest request)
    {
        int managerId = GetCurrentManagerId();

        CreateVehicleCommand command = new CreateVehicleCommand
        {
            ModelId = request.ModelId,
            EnterpriseId = request.EnterpriseId,
            RequestingManagerId = managerId,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            DriverIds = new List<int>(0),
            ActiveDriverId = null,
            AddedToEnterpriseAt = request.AddedToEnterpriseAt
        };

        Result<int> result = await _createHandler.Handle(command);
        if (result.IsFailed)
        {
            return BadRequest();
        }

        return RedirectToAction(nameof(Index), new { enterpriseId = request.EnterpriseId });
    }

    [HttpGet("{id}/edit")]
    public async Task<IActionResult> Edit([FromRoute] int enterpriseId, int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerAndEnterpriseQuery(managerId, enterpriseId);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        VehicleViewModel? viewModel = await viewModelQuery
            .SingleOrDefaultAsync(v => v.Id == id);

        if (viewModel == null)
        {
            return NotFound();
        }

        List<ModelOverview> models = await _context.Models
            .OrderBy(m => m.ModelName)
            .Select(m => new ModelOverview
            {
                Id = m.Id,
                ModelName = m.ModelName
            })
            .ToListAsync();

        ViewBag.Models = models;

        Enterprise? enterprise = await _context.Enterprises
            .FirstOrDefaultAsync(e => e.Id == enterpriseId);

        if (enterprise == null)
        {
            return NotFound();
        }

        ViewBag.Enterprise = new { Id = enterprise.Id, Name = enterprise.Name };

        return View(viewModel);
    }

    [HttpPost("{id}/edit")]
    [AppValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateUpdateVehicleRequest request)
    {
        int managerId = GetCurrentManagerId();

        Vehicle? vehicle = await _context.Vehicles
            .Include(v => v.AssignedDrivers)
            .Include(v => v.ActiveAssignedDriver)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound();
        }

        UpdateVehicleCommand command = new UpdateVehicleCommand()
        {
            Id = id,
            ModelId = request.ModelId,
            EnterpriseId = request.EnterpriseId,
            RequestingManagerId = managerId,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            DriverIds = vehicle.AssignedDrivers.Select(d => d.Id).ToList(),
            ActiveDriverId = vehicle.ActiveAssignedDriver?.Id,
            AddedToEnterpriseAt = vehicle.AddedToEnterpriseAt
        };

        Result<int> result = await _updateHandler.Handle(command);

        // Error flow
        if (result.IsFailed)
        {
            // Errors handling
            return BadRequest();
        }

        // Success flow
        return RedirectToAction(nameof(Edit), new { enterpriseId = command.EnterpriseId, id = id });
    }

    [HttpGet("{id}/delete")]
    public async Task<IActionResult> Delete([FromRoute] int enterpriseId, int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerAndEnterpriseQuery(managerId, enterpriseId);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        VehicleViewModel? viewModel = await viewModelQuery
            .SingleOrDefaultAsync(v => v.Id == id);

        if (viewModel == null)
        {
            return NotFound();
        }

        // Get enterprise details for the view
        Enterprise? enterprise = await _context.Enterprises
            .FirstOrDefaultAsync(e => e.Id == enterpriseId);

        if (enterprise == null)
        {
            return NotFound();
        }

        ViewBag.Enterprise = new { Id = enterprise.Id, Name = enterprise.Name };

        return View(viewModel);
    }

    [HttpPost("{id}/delete")]
    [AppValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed([FromRoute] int enterpriseId, int id)
    {
        int managerId = GetCurrentManagerId();

        DeleteVehicleCommand command = new DeleteVehicleCommand
        {
            Id = id,
            RequestingManagerId = managerId
        };

        Result result = await _deleteHandler.Handle(command);

        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Index), new { enterpriseId = enterpriseId });
        }

        // Errors handling
        return BadRequest();
    }

    private IQueryable<Vehicle> GetFilteredByManagerAndEnterpriseQuery(int managerId, [FromRoute] int enterpriseId)
    {
        IQueryable<int> enterpriseIds = _context.Enterprises
            .Where(e => e.Managers.Any(m => m.Id == managerId) && e.Id == enterpriseId)
            .Select(e => e.Id);

        return _context.Vehicles
            .Where(v => enterpriseIds.Contains(v.EnterpriseId));
    }

    private IQueryable<VehicleViewModel> TransformToViewModelQuery(IQueryable<Vehicle> query)
    {
        IQueryable<VehicleViewModel> vehiclesQuery =
            from v in query
            join e in _context.Enterprises on v.EnterpriseId equals e.Id
            join m in _context.Models on v.ModelId equals m.Id
            select new VehicleViewModel
            {
                Id = v.Id,
                Model = new VehicleViewModel.ModelViewModel
                {
                    Id = m.Id,
                    Name = m.ModelName
                },
                Enterprise = new VehicleViewModel.EnterpriseViewModel
                {
                    Id = e.Id,
                    Name = e.Name
                },
                VinNumber = v.VinNumber,
                Price = v.Price,
                ManufactureYear = v.ManufactureYear,
                Mileage = v.Mileage,
                Color = v.Color,
                AddedToEnterpriseAt = v.AddedToEnterpriseAt
            };

        return vehiclesQuery;
    }

    private int? GetClientTimeZoneOffset()
    {
        // Try to get timezone offset from request headers or cookies
        if (Request.Headers.TryGetValue("X-Client-Timezone-Offset", out Microsoft.Extensions.Primitives.StringValues offsetHeader))
        {
            if (int.TryParse(offsetHeader.FirstOrDefault(), out int offset))
            {
                return offset;
            }
        }

        // Try to get from cookie
        if (Request.Cookies.TryGetValue("client-timezone-offset", out string? offsetCookie))
        {
            if (int.TryParse(offsetCookie, out int offset))
            {
                return offset;
            }
        }

        return null;
    }
}