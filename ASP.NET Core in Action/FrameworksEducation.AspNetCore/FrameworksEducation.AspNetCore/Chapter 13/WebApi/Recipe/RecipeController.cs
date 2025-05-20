using FrameworksEducation.AspNetCore.Chapter_13.WebApi.Filters;
using FrameworksEducation.AspNetCore.Chapter_13.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameworksEducation.AspNetCore.Chapter_13.WebApi.Recipe;

[Authorize]
[FeatureEnabled(IsEnabled = true)]
[ValidateModel]
public class RecipeController : ApiController
{
    private readonly bool IsEnabled = true;

    public RecipeService _service;

    public RecipeController(RecipeService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    [EnsureRecipeExists]
    [AddLastModifiedHeader]
    public IActionResult Get(int id)
    {
        try
        {
            var detail = _service.GetRecipeDetail(id);

            return Ok(detail);
        }
        catch (Exception ex)
        {
            return GetErrorResponse(ex);
        }
    }

    [Authorize("Admin")]
    [HttpPost("{id}")]
    [EnsureRecipeExists]
    public IActionResult Edit(int id, [FromBody] UpdateRecipeCommand command)
    {
        try
        {
            _service.UpdateRecipe(command);

            return Ok();
        }
        catch (Exception ex)
        {
            return GetErrorResponse(ex);
        }
    }

    private static IActionResult GetErrorResponse(Exception ex)
    {
        ProblemDetails error = new ProblemDetails
        {
            Title = "An error occurred",
            Detail = ex.Message,
            Status = 500,
            Type = "https://httpstatuses.com/500"
        };

        return new ObjectResult(error)
        {
            StatusCode = 500
        };
    }
}