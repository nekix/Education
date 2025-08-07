using CarPark.Data;
using CarPark.Models.Enterprises;
using CarPark.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RelatedEntitiesViewModel = CarPark.ViewModels.Api.EnterpriseViewModel.RelatedEntitiesViewModel;
using CarPark.Attributes;
using CarPark.Shared.CQ;
using FluentResults;
using CarPark.Identity;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class EnterprisesController : ApiBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly ICommandHandler<DeleteEnterpriseCommand, Result> _deleteEnterpriseHandler;

    public EnterprisesController(ApplicationDbContext context,
        ICommandHandler<DeleteEnterpriseCommand, Result> deleteEnterpriseHandler)
    {
        _context = context;
        _deleteEnterpriseHandler = deleteEnterpriseHandler;
    }

    // GET: api/Enterprises
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EnterpriseViewModel>))]
    public async Task<ActionResult<IEnumerable<EnterpriseViewModel>>> GetEnterprises()
    {
        int managerId = GetCurrentManagerId();

        IQueryable<Enterprise> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<EnterpriseViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        return Ok(await viewModelQuery.OrderBy(x => x.Id).ToListAsync());
    }

    // GET: api/Enterprises/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnterpriseViewModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnterpriseViewModel>> GetEnterprise(int id)
    {
        int managerId = GetCurrentManagerId();

        IQueryable<Enterprise> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<EnterpriseViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        EnterpriseViewModel? viewModel = await viewModelQuery
            .SingleOrDefaultAsync(x => x.Id == id);

        if (viewModel == null)
        {
            return NotFound();
        }

        return viewModel;
    }

    private IQueryable<Enterprise> GetFilteredByManagerQuery(int managerId)
    {
        IQueryable<Enterprise> filteredQuery =
            from e in _context.Enterprises
            where e.Managers.Any(m => m.Id == managerId)
            select e;

        return filteredQuery;
    }

    private IQueryable<EnterpriseViewModel> TransformToViewModelQuery(IQueryable<Enterprise> query)
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

        IQueryable<EnterpriseViewModel> viewModelQuery =
            from e in query
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

        return viewModelQuery;
    }

    // DELETE: api/Enterprises/5
    [HttpDelete("{id}")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteEnterprise(int id)
    {
        int managerId = GetCurrentManagerId();

        DeleteEnterpriseCommand command = new DeleteEnterpriseCommand
        {
            Id = id,
            RequestingManagerId = managerId
        };

        Result result = await _deleteEnterpriseHandler.Handle(command);

        // Success flow
        if (result.IsSuccess)
            return NoContent();

        // Errors handling
        if (result.HasError(e => e.Message == DeleteEnterpriseCommand.Errors.NotFound))
        {
            return NotFound();
        }

        if (result.HasError(e => e.Message == DeleteEnterpriseCommand.Errors.AccessDenied))
        {
            return Forbid();
        }

        if (result.HasError(e => e.Message == DeleteEnterpriseCommand.Errors.VisibleToOtherManagers) ||
            result.HasError(e => e.Message == DeleteEnterpriseCommand.Errors.HasVehicles) ||
            result.HasError(e => e.Message == DeleteEnterpriseCommand.Errors.HasDrivers) ||
            result.HasError(e => e.Message == DeleteEnterpriseCommand.Errors.Conflict))
        {
            return Conflict();
        }

        // Undefined errors
        return BadRequest();
    }
}