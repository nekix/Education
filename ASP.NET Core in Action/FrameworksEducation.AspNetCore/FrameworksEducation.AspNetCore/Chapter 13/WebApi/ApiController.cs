using Microsoft.AspNetCore.Mvc;

namespace FrameworksEducation.AspNetCore.Chapter_13.WebApi;

[ApiController]
[Route("web-api/[controller]")]
public abstract class ApiController : ControllerBase
{
}