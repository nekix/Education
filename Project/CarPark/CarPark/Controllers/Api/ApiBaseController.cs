using CarPark.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarPark.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public abstract class ApiBaseController : ControllerBase
{
    protected int GetCurrentManagerId()
    {
        string? managerIdText = User.FindFirstValue(AppIdentityConst.ManagerIdClaim);

        return int.Parse(managerIdText!);
    }
}