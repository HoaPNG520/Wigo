using Microsoft.AspNetCore.Mvc;

namespace Wigo.Controllers;

using Microsoft.AspNetCore.Authorization;

public class MemberController : Controller
{
    [AllowAnonymous]
    [HttpGet("Submit/{token}")]
    public IActionResult Submit(string token)
    {
        return RedirectToAction("Submit", "Tracking", new { token });
    }

    [AllowAnonymous]
    [HttpGet("Review/{token}")]
    public IActionResult Review(string token)
    {
        return RedirectToAction("Review", "Tracking", new { token });
    }

    [AllowAnonymous]
    [HttpGet("Success/{token}")]
    public IActionResult Success(string token)
    {
        return RedirectToAction("Success", "Tracking", new { token });
    }

    [AllowAnonymous]
    [HttpGet("Bills/{token}")]
    public IActionResult Bills(string token)
    {
        return RedirectToAction("Bills", "Tracking", new { token });
    }

    [AllowAnonymous]
    [HttpGet("Resubmit/{token}/{id}")]
    public IActionResult Resubmit(string token, int id)
    {
        return RedirectToAction("Resubmit", "Tracking", new { token, expenseId = id });
    }
}