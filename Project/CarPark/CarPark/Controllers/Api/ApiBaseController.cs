using Microsoft.AspNetCore.Mvc;

namespace CarPark.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public abstract class ApiBaseController : ControllerBase
{
}