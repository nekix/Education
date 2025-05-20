using FrameworksEducation.AspNetCore.Chapter_13.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FrameworksEducation.AspNetCore.Chapter_13.WebApi.Filters;

public class EnsureRecipeExistsAttribute : TypeFilterAttribute
{
    public EnsureRecipeExistsAttribute() : base(typeof(EnsureRecipeExistFilter))
    {
    }
}

public class EnsureRecipeExistFilter : IActionFilter
{
    private readonly RecipeService _service;

    public EnsureRecipeExistFilter(RecipeService service)
    {
        _service = service;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {

    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        int recipeId = (int)context.ActionArguments["id"]!;

        if (!_service.DoesRecipeExist(recipeId))
        {
            context.Result = new NotFoundResult();
        }
    }
}