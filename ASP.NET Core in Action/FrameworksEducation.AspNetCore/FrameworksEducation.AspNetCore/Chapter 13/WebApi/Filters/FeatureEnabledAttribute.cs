using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FrameworksEducation.AspNetCore.Chapter_13.WebApi.Filters
{
    public class FeatureEnabledAttribute : Attribute, IResourceFilter
    {
        public bool IsEnabled { get; set; }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!IsEnabled)
            {
                context.Result = new BadRequestResult();
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
