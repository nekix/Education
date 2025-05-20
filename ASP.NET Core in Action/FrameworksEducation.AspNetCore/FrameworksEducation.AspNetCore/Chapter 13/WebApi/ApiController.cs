using FrameworksEducation.AspNetCore.Chapter_13.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FrameworksEducation.AspNetCore.Chapter_13.WebApi;

[ApiController]
[SkipStatusCodePages]
[LogResourceFilter]
[Route("web-api/[controller]")]
public abstract class ApiController : ControllerBase
{
}