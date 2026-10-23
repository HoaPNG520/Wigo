using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wigo.Data;
using Wigo.Models;
using Wigo.Models.Enums;
using Wigo.Models.Entities;
using Wigo.ViewModels;

namespace Wigo.Controllers;

[Authorize(Roles = "FinanceLead,Admin")]
[Route("Reports")]
public class ReportsController : Controller
{
    private readonly UiiDbContext _context;

    public ReportsController(UiiDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        TempData["ActivePage"] = "Reports";
        return View();
    }

    [HttpGet("Generate/{projectId}")]
    public async Task<IActionResult> Generate(int projectId)
    {
        var project = await _context.Projects
            .Include(p => p.Expenses)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
        {
            TempData["Error"] = "Project not found.";
            return RedirectToAction("Index");
        }

        var expenses = project.Expenses.Select(e => new ExpenseSummaryViewModel
        {
            Id = e.Id,
            Amount = e.Amount,
            Status = e.Status,
            SubmittedBy = e.SubmittedBy
        }).ToList();

        var model = new ReportViewModel
        {
            ProjectId = projectId,
            ProjectName = project.Name,
            GeneratedAt = DateTime.UtcNow,
            Expenses = expenses
        };

        return View(model);
    }

    [HttpGet("Export/{projectId}/{format}")]
    public async Task<IActionResult> Export(int projectId, string format)
    {
        var project = await _context.Projects
            .Include(p => p.Expenses)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
        {
            TempData["Error"] = "Project not found.";
            return RedirectToAction("Index");
        }

        var expenses = project.Expenses.Select(e => new ExportExpenseViewModel
        {
            Id = e.Id,
            Amount = e.Amount.Format(),
            Status = e.Status,
            SubmittedBy = e.SubmittedBy,
            SubmittedAt = e.SubmittedAt
        }).ToList();

        format = format.ToLowerInvariant();

        if (format == "csv")
        {
            var csv = $"Project: {project.Name}\n";
            csv += "ID,Amount,Status,SubmittedBy,SubmittedAt\n";
            foreach (var exp in expenses)
            {
                csv += $"{exp.Id},{exp.Amount},{exp.Status},{exp.SubmittedBy},{exp.SubmittedAt:yyyy-MM-dd}\n";
            }
            TempData["Success"] = "CSV export generated successfully.";
            return Content(csv, "text/csv");
        }

        if (format == "pdf")
        {
            TempData["Success"] = "PDF export generated successfully.";
            return Content($"PDF export for project {projectId}", "application/pdf");
        }

        TempData["Error"] = "Invalid format. Use 'csv' or 'pdf'.";
        return RedirectToAction("Generate", new { projectId });
    }
}