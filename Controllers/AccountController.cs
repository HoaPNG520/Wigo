using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wigo.Data;
using Wigo.Models.Entities;
using System.Security.Claims;

namespace Wigo.Controllers;

[Authorize(Roles = "FinanceLead,Admin")]
[Route("Account")]
public class AccountController : Controller
{
    private readonly UiiDbContext _context;

    public AccountController(UiiDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("Login")]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Projects");
        }
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || user.PasswordHash != password)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError("", "Account is disabled.");
            return View();
        }

        var claims = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, user.Email), new Claim("role", user.Role.ToString()) },
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claims));

        return LocalRedirect("/Projects/Index");
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View();
    }
}