using CarPark.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarPark.Controllers.Api.Controllers;

[Route("api/[controller]")]
[Area("Api")]
[ApiController]
[Produces("application/json")]
public abstract class ApiBaseController : ControllerBase
{
    protected Guid GetCurrentManagerId()
    {
        string? managerIdText = User.FindFirstValue(AppIdentityConst.ManagerIdClaim);

        return Guid.Parse(managerIdText!);
    }
}