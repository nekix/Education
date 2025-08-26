using CarPark.Controllers.Api.Abstract;
using CarPark.Data;
using CarPark.Identity;
using CarPark.Models.Drivers;
using CarPark.ViewModels.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using VehiclesAssignmentsViewModel = CarPark.ViewModels.Api.DriverViewModel.VehiclesAssignmentsViewModel;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class DriversController : ApiBaseController
{
    private readonly ApplicationDbContext _context;

    public DriversController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Drivers
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DriverViewModel>))]
    public async Task<ActionResult<GetDriversResponse>> GetDrivers([FromQuery] GetDriversRequest request)
    {
        int managerId = GetCurrentManagerId();

        IQueryable<Driver> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<Driver> orderedQuery = originalQuery
            .OrderBy(x => x.EnterpriseId)
            .ThenBy(x => x.FullName);

        uint total = (uint)orderedQuery.Count();

        IQueryable<Driver> paginatedQuery = orderedQuery
            .Skip((int)request.Offset)
            .Take((int)request.Limit);

        IQueryable<DriverViewModel> viewModelQuery = TransformToViewModelQuery(paginatedQuery);

        List<DriverViewModel> viewModels = await viewModelQuery
            .OrderBy(x => x.EnterpriseId)
            .ThenBy(x => x.FullName)
            .ToListAsync();

        GetDriversResponse response = new GetDriversResponse
        {
            Data = viewModels,
            Meta = new GetDriversResponse.Metadata()
            {
                Limit = request.Limit,
                Offset = request.Offset,
                Total = total
            }
        };

        return Ok(response);
    }

    // GET: api/Drivers/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DriverViewModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DriverViewModel>> GetDriver(int id)
    {
        int managerId = GetCurrentManagerId();

        IQueryable<Driver> originalQuery = GetFilteredByManagerQuery(managerId);

        IQueryable<DriverViewModel> viewModelQuery = TransformToViewModelQuery(originalQuery);

        DriverViewModel? viewModel = await viewModelQuery
            .SingleOrDefaultAsync(x => x.Id == id);

        if (viewModel == null)
        {
            return NotFound();
        }

        return viewModel;
    }

    public class GetDriversRequest : IPaginationRequest
    {
        [Required]
        [Range(1, 1000)]
        public required uint Limit { get; init; }

        [Required]
        [Range(0, int.MaxValue)]
        public required uint Offset { get; init; }
    }

    public class GetDriversResponse : IPaginationModel<DriverViewModel, GetDriversResponse.Metadata>
    {
        public required Metadata Meta { get; init; }

        public required IEnumerable<DriverViewModel> Data { get; init; }

        public class Metadata : IPaginationMetadata
        {
            public uint Offset { get; init; }

            public uint Limit { get; init; }

            public uint Total { get; init; }
        }
    }

    private IQueryable<Driver> GetFilteredByManagerQuery(int managerId)
    {
        IQueryable<int> enterpriseIds = _context.Enterprises
            .Where(e => e.Managers.Any(m => m.Id == managerId))
            .Select(e => e.Id);

        return _context.Drivers
            .Where(v => enterpriseIds.Contains(v.EnterpriseId));
    }

    private static IQueryable<DriverViewModel> TransformToViewModelQuery(IQueryable<Driver> query)
    {
        IQueryable<DriverViewModel> viewModelQuery =
            from d in query
            from v in d.AssignedVehicles.DefaultIfEmpty()
            group v by new
            {
                d.Id,
                d.EnterpriseId,
                d.FullName,
                d.DriverLicenseNumber,
                ActiveVehicleId = d.ActiveAssignedVehicle != null ? d.ActiveAssignedVehicle.Id : (int?)null
            } into g
            select new DriverViewModel
            {
                Id = g.Key.Id,
                EnterpriseId = g.Key.EnterpriseId,
                FullName = g.Key.FullName,
                DriverLicenseNumber = g.Key.DriverLicenseNumber,
                VehiclesAssignments = new VehiclesAssignmentsViewModel
                {
                    VehiclesIds = EF.Functions.ArrayAgg(
                        g.Where(x => x != null)
                            .Select(x => x.Id)
                            .Order()),
                    ActiveVehicleId = g.Key.ActiveVehicleId,
                }
            };

        return viewModelQuery;
    }
}