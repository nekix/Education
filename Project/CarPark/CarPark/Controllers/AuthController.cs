using System.Security.Claims;
using CarPark.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using CarPark.Attributes;
using CarPark.Identity;
using CarPark.Models.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace CarPark.Controllers;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(ApplicationDbContext context, 
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        
        _signInManager = signInManager;
        _signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        _userManager = userManager;
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
        {
            return View();
        }

        SignInResult result =
            await _signInManager.PasswordSignInAsync(request.Username, request.Password, true, lockoutOnFailure: true);

        //SignInResult result = await PasswordSignInAsync(request.Username, request.Password, true, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", result.ToString());
            ViewData["ReturnUrl"] = request.ReturnUrl;
            return View();
        }

        IdentityUser user = (await _userManager.FindByNameAsync(request.Username))!;

        Manager? manager = await _context.Managers.FirstOrDefaultAsync(m => m.IdentityUserId == user.Id);
        if (manager != null)
            await _userManager.AddClaimAsync(user, new Claim(AppIdentityConst.ManagerIdClaim, manager.Id.ToString()));

        // The signInManager already produced the needed response in the form of a cookie or bearer token.
        if (request.ReturnUrl != null)
        { 
            return Redirect(request.ReturnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [AppValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await HttpContext.SignOutAsync(_signInManager.AuthenticationScheme);

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