using Microsoft.AspNetCore.Mvc.Filters;

namespace FrameworksEducation.AspNetCore.Chapter_13.Services;

public class LogResourceFilter : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        Console.WriteLine("Executing!");
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        Console.WriteLine("Executed!");
    }
}