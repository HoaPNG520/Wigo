using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wigo.Data;
using Wigo.Models.Entities;

namespace Wigo.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly UiiDbContext _context;

    public HomeController(UiiDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}