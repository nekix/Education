using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CarPark.TagHelpers;

[HtmlTargetElement("*", Attributes = "asp-authorize-policy")]
public class AuthorizeTagHelper : TagHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizationService _authorizationService;

    public AuthorizeTagHelper(
        IHttpContextAccessor httpContextAccessor, 
        IAuthorizationService authorizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizationService = authorizationService;
    }

    [HtmlAttributeName("asp-authorize-policy")]
    public string? Policy { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await base.ProcessAsync(context, output);

        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;

        if (user == null)
        {
            output.SuppressOutput();
            return;
        }

        if (Policy != null)
        {
            bool authorized = (await _authorizationService.AuthorizeAsync(user, Policy)).Succeeded;

            if (!authorized)
            {
                output.SuppressOutput();
            }
        }
    }
}