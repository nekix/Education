using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CarPark.Attributes;
using CarPark.Data;
using CarPark.Identity;
using CarPark.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CarPark.Controllers.Api.Controllers;

public class AuthController : ApiBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(SignInManager<IdentityUser> signInManager, ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        SignInResult result = await _signInManager.PasswordSignInAsync(
            request.Username,
            request.Password,
            true,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-401-unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = result.ToString()
            });
        }

        return Created();
    }

    [HttpPost("logout")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return NoContent();
    }

    public class LoginRequest
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}