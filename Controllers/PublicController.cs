using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wigo.Data;
using Wigo.Models.Entities;
using Wigo.Models.ValueObjects;
using Wigo.Services;
using Wigo.ViewModels;

namespace Wigo.Controllers;

[AllowAnonymous]
[Route("Public")]
public class PublicController : Controller
{
    private readonly IProjectRepository _projectRepo;

    public PublicController(IProjectRepository projectRepo)
    {
        _projectRepo = projectRepo;
    }

    [HttpGet("Portal/{projectId}")]
    public async Task<IActionResult> Portal(int projectId)
    {
        var project = await _projectRepo.GetByIdAsync(projectId);
        if (project == null)
        {
            return NotFound();
        }

        var model = new PublicPortalViewModel
        {
            ProjectId = project.Id,
            Title = project.Name,
            Budget = project.Budget.Format(),
            BaseSpent = project.BaseSpent.Format(),
            Status = project.Status,
            PublicEnabled = project.PublicEnabled,
            SubmissionsOpen = project.SubmissionsOpen
        };

        return View(model);
    }

    [HttpGet("Expense/{projectId}/{expenseId}")]
    public async Task<IActionResult> Expense(int projectId, int expenseId)
    {
        var project = await _projectRepo.GetByIdAsync(projectId);
        if (project == null)
        {
            return NotFound("Project not found");
        }

        var expense = project.Expenses.FirstOrDefault(e => e.Id == expenseId);
        if (expense == null || expense.ProjectId != projectId)
        {
            return NotFound("Expense not found");
        }

        var model = new PublicExpenseViewModel
        {
            ProjectId = project.Id,
            ExpenseId = expense.Id,
            MaskedAmount = "₫***" ,
            Description = "Expense details masked for public viewing"
        };

        return View(model);
    }

    [HttpGet("Offline/{projectId}")]
    public async Task<IActionResult> Offline(int projectId)
    {
        var project = await _projectRepo.GetByIdAsync(projectId);
        if (project == null)
        {
            return NotFound();
        }

        var model = new PublicOfflineViewModel
        {
            ProjectId = project.Id,
            Title = project.Name
        };

        return View(model);
    }
}