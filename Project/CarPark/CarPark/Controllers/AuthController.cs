using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using CarPark.Attributes;

namespace CarPark.Controllers;

public class AuthController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
        _signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AppValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromForm] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return View();

        SignInResult result = await _signInManager.PasswordSignInAsync(
                request.Username, 
                request.Password, 
                true, 
                lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Username or password is invalid");
            ViewData["ReturnUrl"] = request.ReturnUrl;
            return View();
        }

        if (!string.IsNullOrEmpty(request.ReturnUrl) && Url.IsLocalUrl(request.ReturnUrl))
            return Redirect(request.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [AppValidateAntiForgeryToken]
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

        public required string? ReturnUrl { get; set; }
    }
}  