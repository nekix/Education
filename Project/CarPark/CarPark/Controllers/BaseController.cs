using CarPark.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarPark.Controllers;

public class BaseController : Controller
{
    protected Guid GetCurrentManagerId()
    {
        string? managerIdText = User.FindFirstValue(AppIdentityConst.ManagerIdClaim);

        return Guid.Parse(managerIdText!);
    }
}