using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameworksEducation.AspNetCore.Chapter_13.WebApi.Auth
{
    [AllowAnonymous]
    public class AuthController : ApiController
    {
        [HttpGet("login")]
        public async Task<IActionResult> LoginAsync(bool isAdmin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "universal"),
            };

            if (isAdmin )
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok();
        }
    }
}
