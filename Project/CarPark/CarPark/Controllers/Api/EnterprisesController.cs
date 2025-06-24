using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.Models;
using CarPark.ViewModels.Api;
using DriverViewModel = CarPark.ViewModels.Api.EnterpriseOverviewViewModel.DriverViewModel;
using DriverAssignmentViewModel = CarPark.ViewModels.Api.EnterpriseOverviewViewModel.VehicleViewModel.DriverAssignmentViewModel;
using VehicleViewModel = CarPark.ViewModels.Api.EnterpriseOverviewViewModel.VehicleViewModel;
using VehicleAssignmentViewModel = CarPark.ViewModels.Api.EnterpriseOverviewViewModel.DriverViewModel.VehicleAssignmentViewModel;

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
        //var driversQuery =
        //    from d in _context.Drivers
        //    select new
        //    {
        //        d.EnterpriseId,
        //        ViewModel = new DriverViewModel
        //        {
        //            Id = d.Id,
        //            FullName = d.FullName,
        //            DriverLicenseNumber = d.DriverLicenseNumber,
        //            VehiclesAssignments = (
        //                from g in _context.DriverVehicleAssignments
        //                where g.DriverId == d.Id
        //                select new VehicleAssignmentViewModel
        //                {
        //                    IsActive = g.IsActiveAssignment,
        //                    VehicleId = g.VehicleId
        //                }).ToList()
        //        }
        //    };

        //var vehiclesQuery =
        //    from v in _context.Vehicles
        //    select new
        //    {
        //        v.EnterpriseId,
        //        ViewModel = new VehicleViewModel
        //        {
        //            Id = v.Id,
        //            ModelId = v.ModelId,
        //            VinNumber = v.VinNumber,
        //            Price = v.Price,
        //            ManufactureYear = v.ManufactureYear,
        //            Mileage = v.Mileage,
        //            Color = v.Color,
        //            DriverAssignments = (
        //                from a in _context.DriverVehicleAssignments
        //                where a.VehicleId == v.Id
        //                select new DriverAssignmentViewModel
        //                {
        //                    IsActive = a.IsActiveAssignment,
        //                    DriverId = a.DriverId
        //                }).ToList()
        //        }
        //    };

        //IQueryable<EnterpriseOverviewViewModel> overviewQuery =
        //    from e in _context.Enterprises
        //    let drivers = driversQuery
        //        .Where(x => x.EnterpriseId == e.Id)
        //        .Select(x => x.ViewModel)
        //        .ToList()
        //    let vehicles = vehiclesQuery
        //        .Where(x => x.EnterpriseId == e.Id)
        //        .Select(x => x.ViewModel)
        //        .ToList()
        //    select new EnterpriseOverviewViewModel
        //    {
        //        Id = e.Id,
        //        Name = e.Name,
        //        LegalAddress = e.LegalAddress,
        //        Drivers = drivers,
        //        Vehicles = vehicles
        //    };

        var driversQuery =
            from d in _context.Drivers
            select new
            {
                d.EnterpriseId,
                ViewModel = new DriverViewModel
                {
                    Id = d.Id,
                    VehiclesAssignments = (
                        from g in _context.DriverVehicleAssignments
                        where g.DriverId == d.Id
                        select new VehicleAssignmentViewModel
                        {
                            IsActive = g.IsActiveAssignment,
                            VehicleId = g.VehicleId
                        }).ToList()
                }
            };

        IQueryable<EnterpriseOverviewViewModel> overviewQuery =
            from e in _context.Enterprises
            let drivers = driversQuery
                .Where(x => x.EnterpriseId == e.Id)
                .Select(x => x.ViewModel)
                .ToList()
            select new EnterpriseOverviewViewModel
            {
                Id = e.Id,
                Name = e.Name,
                LegalAddress = e.LegalAddress,
                Drivers = drivers
            };

        return Ok(await overviewQuery.FirstOrDefaultAsync(q => q.Id == id));
    }
}