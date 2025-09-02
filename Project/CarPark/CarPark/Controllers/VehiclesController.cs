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

namespace CarPark.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class VehiclesController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly ICommandHandler<DeleteVehicleCommand, Result> _deleteHandler;
    private readonly ICommandHandler<UpdateVehicleCommand, Result<int>> _updateHandler;
    private readonly ICommandHandler<CreateVehicleCommand, Result<int>> _createHandler;

    public VehiclesController(ApplicationDbContext context,
        ICommandHandler<DeleteVehicleCommand, Result> deleteHandler,
        ICommandHandler<UpdateVehicleCommand, Result<int>> updateHandler,
        ICommandHandler<CreateVehicleCommand, Result<int>> createHandler)
    {
        _context = context;
        _deleteHandler = deleteHandler;
        _updateHandler = updateHandler;
        _createHandler = createHandler;
    }

    public async Task<IActionResult> Index()
    {
        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        List<VehicleViewModel> viewModels = await viewModelQuery.ToListAsync();

        return View(viewModels);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        VehicleViewModel? viewModel = await viewModelQuery
            .SingleOrDefaultAsync(v => v.Id == id);

        if (viewModel == null)
        {
            return NotFound();
        }

        return View(viewModel);
    }

    public async Task<IActionResult> Create(int? enterpriseId)
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

        List<EnterpriseOverview> enterprises = await _context.Enterprises
            .Where(e => e.Managers.Any(m => m.Id == managerId))
            .OrderBy(e => e.Name)
            .Select(e => new EnterpriseOverview
            {
                Id = e.Id,
                EnterpriseName = e.Name
            })
            .ToListAsync();

        ViewBag.Enterprises = enterprises;
        
        ViewBag.SelectedEnterpriseId = enterpriseId;

        return View();
    }

    [HttpPost]
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
            ActiveDriverId = null
        };

        Result<int> result = await _createHandler.Handle(command);
        if (result.IsFailed)
        {
            return BadRequest();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerQuery(managerId);

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

        List<EnterpriseOverview> enterprises = await _context.Enterprises
            .Where(e => e.Managers.Any(m => m.Id == managerId))
            .OrderBy(e => e.Name)
            .Select(e => new EnterpriseOverview
            {
                Id = e.Id,
                EnterpriseName = e.Name
            })
            .ToListAsync();

        ViewBag.Enterprises = enterprises;

        return View(viewModel);
    }

    [HttpPost]
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
            ActiveDriverId = vehicle.ActiveAssignedDriver?.Id
        };

        Result<int> result = await _updateHandler.Handle(command);

        // Error flow
        if (result.IsFailed)
        {
            // Errors handling
            return BadRequest();
        }

        // Success flow
        return await Edit(id);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        int managerId = GetCurrentManagerId();

        IQueryable<Vehicle> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<VehicleViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        VehicleViewModel? viewModel = await viewModelQuery
            .SingleOrDefaultAsync(v => v.Id == id);

        if (viewModel == null)
        {
            return NotFound();
        }

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [AppValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
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
            return RedirectToAction(nameof(Index));
        }

        // Errors handling
        return BadRequest();
    }

    private IQueryable<Vehicle> GetFilteredByManagerQuery(int managerId)
    {
        IQueryable<int> enterpriseIds = _context.Enterprises
            .Where(e => e.Managers.Any(m => m.Id == managerId))
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
                Color = v.Color
            };

        return vehiclesQuery;
    }
}