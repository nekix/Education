using CarPark.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CarPark.Identity;
using CarPark.Models;
using Microsoft.Build.Framework;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromForm] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        SignInResult result = await PasswordSignInAsync(request.Username, request.Password, true, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", result.ToString());
            ViewData["ReturnUrl"] = request.ReturnUrl;
            return View();
        }

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
    [ValidateAntiForgeryToken]
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

    private async Task<SignInResult> PasswordSignInAsync(
        string userName,
        string password,
        bool isPersistent, 
        bool lockoutOnFailure)
    {
        IdentityUser? user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return SignInResult.Failed;
        }

        SignInResult attempt = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        if (!attempt.Succeeded)
        {
            return attempt;
        }

        int managerId = await _context.Managers
            .Where(m => m.IdentityUserId == user!.Id)
            .Select(m => m.Id)
            .SingleAsync();

        await _signInManager.SignInWithClaimsAsync(user, isPersistent, 
            new Claim[]
            {
                new Claim("amr", "pwd"),
                new Claim(AppIdentityConst.ManagerIdClaim, managerId.ToString())
            });

        return SignInResult.Success;
    }
} 