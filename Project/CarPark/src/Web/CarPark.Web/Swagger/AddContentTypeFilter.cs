using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CarPark.Swagger;

public class AddContentTypeFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var responses = operation.Responses;
        foreach (var response in responses)
        {
            response.Value.Content.Add("*/*", new OpenApiMediaType());
        }
    }
}