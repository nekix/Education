﻿using CarPark.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarPark.Areas.Api.Api;

[Route("api/[controller]")]
[Area("Api")]
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