using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace CarPark.Controllers;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Введите логин и пароль");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        User? user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);

        if (user == null)
        {
            ModelState.AddModelError("", "Неверный логин или пароль");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(claimsPrincipal);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }
} 