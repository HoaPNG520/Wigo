using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wigo.Data;
using Wigo.Models;
using Wigo.Models.Entities;
using Wigo.Models.Enums;
using Wigo.Services;
using Wigo.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Wigo.Controllers;

[Authorize(Roles = "FinanceLead,Admin")]
[Route("Expenses")]
public class ExpensesController : Controller
{
    private readonly IExpenseRepository _expenseRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly UiiDbContext _context;

    public ExpensesController(IExpenseRepository expenseRepo, IProjectRepository projectRepo, UiiDbContext context)
    {
        _expenseRepo = expenseRepo;
        _projectRepo = projectRepo;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? status, int? projectId, DateTime? startDate, DateTime? endDate)
    {
        var expenses = await _expenseRepo.GetAllAsync();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse(status, out ExpenseStatus parsedStatus))
        {
            expenses = expenses.Where(e => e.Status == parsedStatus).ToList();
        }

        if (projectId.HasValue)
        {
            expenses = expenses.Where(e => e.ProjectId == projectId.Value).ToList();
        }

        if (startDate.HasValue)
        {
            expenses = expenses.Where(e => e.SubmittedAt >= startDate.Value).ToList();
        }

        if (endDate.HasValue)
        {
            expenses = expenses.Where(e => e.SubmittedAt <= endDate.Value).ToList();
        }

        var statusList = Enum.GetNames<ExpenseStatus>().ToList();
        var projects = await _projectRepo.GetAllAsync();
        var model = new ExpenseIndexViewModel
        {
            Expenses = expenses.OrderByDescending(e => e.SubmittedAt).ToList(),
            StatusFilters = statusList,
            StatusFilter = status,
            Projects = projects.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList(),
            ProjectFilter = projectId,
            StartDate = startDate,
            EndDate = endDate
        };

        return View(model);
    }

    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var expense = await _expenseRepo.GetByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }
        return View(expense);
    }

    [HttpGet("Create")]
    [Authorize(Roles = "FinanceLead,Admin")]
    public async Task<IActionResult> Create()
    {
        var projects = await _projectRepo.GetAllAsync();
        var model = new ExpenseCreateViewModel
        {
            Projects = projects.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList()
        };
        return View(model);
    }

    [HttpPost("Create")]
    [Authorize(Roles = "FinanceLead,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseCreateViewModel model, IFormFile? uploadedFile)
    {
        if (!ModelState.IsValid)
        {
            var projects = await _projectRepo.GetAllAsync();
            model.Projects = projects.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();
            return View(model);
        }

        var expense = new Expense
        {
            ProjectId = model.ProjectId,
            SubmittedBy = User.Identity?.Name ?? "",
            SubmittedAt = DateTime.UtcNow,
            Status = ExpenseStatus.Pending,
            CategoryId = model.CategoryId
        };

        await _expenseRepo.AddAsync(expense);

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

        return RedirectToAction("Index");
    }

    [HttpGet("Edit/{id}")]
    [Authorize(Roles = "FinanceLead,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var expense = await _expenseRepo.GetByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        var projects = await _projectRepo.GetAllAsync();
        var model = new ExpenseEditViewModel
        {
            Id = expense.Id,
            ProjectId = expense.ProjectId,
            CategoryId = expense.CategoryId,
            Amount = expense.Amount.Amount,
            Status = expense.Status,
            SubmittedBy = expense.SubmittedBy,
            ReviewNotes = expense.ReviewNotes,
            Projects = projects.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList(),
            Categories = new List<SelectListItem>()
        };

        return View(model);
    }

    [HttpPost("Edit/{id}")]
    [Authorize(Roles = "FinanceLead,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExpenseEditViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            var projects = await _projectRepo.GetAllAsync();
            model.Projects = projects.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();
            model.Categories = new List<SelectListItem>();
            return View(model);
        }

        var expense = await _expenseRepo.GetByIdAsync(model.Id);
        if (expense == null)
        {
            return NotFound();
        }

        expense.ProjectId = model.ProjectId;
        expense.CategoryId = model.CategoryId;
        expense.AiExtractedFields = model.AiExtractedFields;
        expense.ReviewNotes = model.ReviewNotes;

        await _expenseRepo.UpdateAsync(expense);

        return RedirectToAction("Index");
    }

    [HttpPost("Delete/{id}")]
    [Authorize(Roles = "FinanceLead,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _expenseRepo.DeleteAsync(id);
        return RedirectToAction("Index");
    }

    [HttpPost("Approve/{id}")]
    [Authorize(Roles = "FinanceLead,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var expense = await _expenseRepo.GetByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        expense.Status = ExpenseStatus.Recorded;
        expense.ReviewedBy = (User.Identity?.Name ?? "").GetHashCode();
        expense.ReviewedAt = DateTime.UtcNow;
        expense.ReviewNotes = "Approved by admin";

        await _expenseRepo.UpdateAsync(expense);

        return RedirectToAction("Index");
    }

    [HttpPost("Reject/{id}", Name = "Reject")]
    [Authorize(Roles = "FinanceLead,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string rejectReason)
    {
        var expense = await _expenseRepo.GetByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        expense.Status = ExpenseStatus.Rejected;
        expense.ReviewedBy = (User.Identity?.Name ?? "").GetHashCode();
        expense.ReviewedAt = DateTime.UtcNow;
        expense.ReviewNotes = rejectReason ?? "Rejected by admin";

        await _expenseRepo.UpdateAsync(expense);

        return RedirectToAction("Index");
    }

    [HttpPost("NeedsMore/{id}")]
    [Authorize(Roles = "FinanceLead,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NeedsMore(int id, string moreInfoRequest)
    {
        var expense = await _expenseRepo.GetByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        expense.Status = ExpenseStatus.NeedsMore;
        expense.ReviewedBy = (User.Identity?.Name ?? "").GetHashCode();
        expense.ReviewedAt = DateTime.UtcNow;
        expense.ReviewNotes = moreInfoRequest ?? "More information requested";

        await _expenseRepo.UpdateAsync(expense);

        return RedirectToAction("Index");
    }
}