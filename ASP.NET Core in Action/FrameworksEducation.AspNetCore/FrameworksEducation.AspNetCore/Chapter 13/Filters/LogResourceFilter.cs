using Microsoft.AspNetCore.Mvc.Filters;

namespace FrameworksEducation.AspNetCore.Chapter_13.Filters;

public class LogResourceFilter : Attribute, IResourceFilter
{
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        Console.WriteLine("Executing!");
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        Console.WriteLine("Executed!");
    }
}