using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wigo.Data;
using Wigo.Models;
using Wigo.Models.Entities;
using Wigo.Models.ValueObjects;
using Wigo.Services;
using Wigo.ViewModels;

namespace Wigo.Controllers;

[AllowAnonymous]
[Route("Tracking")]
public class TrackingController : Controller
{
    private readonly UiiDbContext _context;
    private readonly ITrackingTokenRepository _tokenRepo;

    public TrackingController(UiiDbContext context, ITrackingTokenRepository tokenRepo)
    {
        _context = context;
        _tokenRepo = tokenRepo;
    }

    [HttpGet("Submit/{token}")]
    public async Task<IActionResult> Submit(string token)
    {
        var trackingToken = await _tokenRepo.GetByValueAsync(token);

        if (trackingToken == null || trackingToken.IsExpired())
        {
            return NotFound("Invalid or expired tracking token.");
        }

        var model = new TrackingSubmitViewModel
        {
            Token = token,
            Expense = new Expense
            {
                SubmittedAt = DateTime.UtcNow,
                Status = ExpenseStatus.Pending
            }
        };

        return View(model);
    }

    [HttpPost("Submit/{token}")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(string token, TrackingSubmitViewModel model, IFormFile? uploadedFile)
    {
        var trackingToken = await _tokenRepo.GetByValueAsync(token);

        if (trackingToken == null || trackingToken.IsExpired())
        {
            return NotFound("Invalid or expired tracking token.");
        }

        if (ModelState.IsValid)
        {
            var expense = new Expense
            {
                ProjectId = model.Expense.ProjectId,
                SubmittedBy = model.Expense.SubmittedBy,
                Amount = model.Expense.Amount,
                Status = ExpenseStatus.Pending,
                SubmittedAt = DateTime.UtcNow,
                TrackingTokenId = trackingToken.Id
            };

            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();

            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                var evidence = new EvidenceFile
                {
                    ExpenseId = expense.Id,
                    FileName = uploadedFile.FileName,
                    MimeType = uploadedFile.ContentType,
                    BlobUrl = "/evidence/" + expense.Id + "/" + Guid.NewGuid(),
                    FileSize = uploadedFile.Length,
                    RawText = "{}",
                    ExtractedFields = "{}"
                };
                _context.EvidenceFiles.Add(evidence);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Expense submitted successfully.";
            return RedirectToAction("Success", new { token });
        }

        model.Token = token;
        return View(model);
    }

    [HttpGet("Review/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> Review(string token)
    {
        var trackingToken = await _tokenRepo.GetByValueAsync(token);

        if (trackingToken == null || trackingToken.IsExpired())
        {
            return NotFound("Invalid or expired tracking token.");
        }

        var expenses = await _context.Expenses
            .Where(e => e.TrackingTokenId == trackingToken.Id)
            .ToListAsync();

        var model = new TrackingReviewViewModel
        {
            Token = token,
            Expenses = expenses
        };

        return View(model);
    }

    [HttpGet("Success/{token}")]
    [AllowAnonymous]
    public IActionResult Success(string token)
    {
        ViewData["Token"] = token;
        return View();
    }

    [HttpGet("Bills/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> Bills(string token)
    {
        var trackingToken = await _tokenRepo.GetByValueAsync(token);

        if (trackingToken == null || trackingToken.IsExpired())
        {
            return NotFound("Invalid or expired tracking token.");
        }

        var expenses = await _context.Expenses
            .Where(e => e.TrackingTokenId == trackingToken.Id && e.Status == ExpenseStatus.Recorded)
            .ToListAsync();

        var model = new TrackingBillsViewModel
        {
            Token = token,
            Expenses = expenses
        };

        return View(model);
    }

    [HttpGet("Resubmit/{token}/{expenseId}")]
    [AllowAnonymous]
    public async Task<IActionResult> Resubmit(string token, int expenseId)
    {
        var trackingToken = await _tokenRepo.GetByValueAsync(token);

        if (trackingToken == null || trackingToken.IsExpired())
        {
            return NotFound("Invalid or expired tracking token.");
        }

        var expense = await _context.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.TrackingTokenId == trackingToken.Id);

        if (expense == null)
        {
            return NotFound("Expense not found or access denied.");
        }

        var model = new TrackingResubmitViewModel
        {
            Token = token,
            ExpenseId = expenseId,
            Expense = expense
        };

        return View(model);
    }
}