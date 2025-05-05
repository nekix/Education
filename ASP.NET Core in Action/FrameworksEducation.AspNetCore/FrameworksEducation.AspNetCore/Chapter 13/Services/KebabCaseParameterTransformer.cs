using System.Text.RegularExpressions;

namespace FrameworksEducation.AspNetCore.Chapter_13.Services;

public partial class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        string? route = value?.ToString();

        if (route == null) return null;

        return MyRegex().Replace(route, "$1-$2".ToLower());
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex MyRegex();
}