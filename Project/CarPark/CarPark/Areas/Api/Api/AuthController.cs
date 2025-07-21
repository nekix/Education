using System.ComponentModel.DataAnnotations;
using CarPark.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CarPark.Areas.Api.Api;

public class AuthController : ApiBaseController
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        SignInResult result =
            await _signInManager.PasswordSignInAsync(request.Username, request.Password, true, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-401-unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = result.ToString()
            });
        }

        return Ok();
    }

    [HttpPost("logout")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await _signInManager.SignOutAsync();

        if (returnUrl != null)
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    public class LoginRequest
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}