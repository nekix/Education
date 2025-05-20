using FrameworksEducation.AspNetCore.Chapter_13.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FrameworksEducation.AspNetCore.Chapter_13.WebApi.Filters
{
    public class AddLastModifiedHeaderAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is OkObjectResult result
                && result.Value is RecipeDetailViewModel detail)
            {
                DateTime viewModelDate = detail.LastModified;
                context.HttpContext.Response.GetTypedHeaders().LastModified = viewModelDate;
            }
        }
    }
}
