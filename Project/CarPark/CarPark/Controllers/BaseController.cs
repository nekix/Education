using CarPark.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarPark.Controllers;

public class BaseController : Controller
{
    protected int GetCurrentManagerId()
    {
        string? managerIdText = User.FindFirstValue(AppIdentityConst.ManagerIdClaim);

        return int.Parse(managerIdText!);
    }
}