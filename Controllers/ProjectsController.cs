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
[Route("Projects")]
public class ProjectsController : Controller
{
    private readonly IProjectRepository _projectRepo;
    private readonly UiiDbContext _context;

    public ProjectsController(IProjectRepository projectRepo, UiiDbContext context)
    {
        _projectRepo = projectRepo;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var projects = await _projectRepo.GetAllAsync();
        var model = projects.Select(p => new ProjectIndexViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Status = (int)p.Status,
            Budget = new Money(p.Budget.Amount, "VND"),
            BaseSpent = new Money(p.BaseSpent.Amount, "VND"),
            PublicEnabled = p.PublicEnabled,
            SubmissionsOpen = p.SubmissionsOpen,
            ExpenseCount = p.Expenses.Count,
            CreatedAt = p.CreatedAt
        }).OrderByDescending(p => p.CreatedAt).ToList();

        return View(model);
    }

    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var project = await _projectRepo.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        var categories = await _context.BudgetCategories
            .Where(c => c.ProjectId == project.Id)
            .ToListAsync();

        var model = new ProjectDetailsViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Status = (int)project.Status,
            OrganizationName = project.Organization?.Name ?? "",
            Budget = project.Budget,
            BaseSpent = project.BaseSpent,
            PublicEnabled = project.PublicEnabled,
            SubmissionsOpen = project.SubmissionsOpen,
            Categories = categories.Select(c => new CategoryBudgetViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                BudgetAllocated = c.TotalAllocated.Amount,
                SpentAmount = c.Spent.Amount
            }).ToList(),
            Expenses = project.Expenses.Select(e => new ExpenseSummaryViewModel
            {
                Id = e.Id,
                Amount = e.Amount,
                Status = e.Status,
                SubmittedBy = e.SubmittedBy
            }).ToList()
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new ProjectCreateViewModel
        {
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddYears(1),
            Categories = new List<BudgetCategoryViewModel>()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = new List<BudgetCategoryViewModel>();
            return View(model);
        }

        var project = new Project
        {
            Name = model.Name,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.BudgetObj,
            Status = (ProjectStatus)model.Status,
            PublicEnabled = model.PublicEnabled,
            SubmissionsOpen = model.SubmissionsOpen,
            CreatedAt = DateTime.UtcNow
        };

        await _projectRepo.AddAsync(project);

        if (model.CategoryNames != null)
        {
            foreach (var catName in model.CategoryNames)
            {
                if (!string.IsNullOrEmpty(catName))
                {
                    var category = new BudgetCategory
                    {
                        ProjectId = project.Id,
                        Name = catName,
                        Color = "#" + Guid.NewGuid().ToString().Substring(0, 6),
                        SortOrder = 0
                    };
                    _context.BudgetCategories.Add(category);
                }
            }
            await _context.SaveChangesAsync();
        }

        TempData["Success"] = "Project created successfully.";
        return RedirectToAction("Index");
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var project = await _projectRepo.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        var model = new ProjectEditViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Status = (int)project.Status,
            Budget = project.Budget.Amount,
            PublicEnabled = project.PublicEnabled,
            SubmissionsOpen = project.SubmissionsOpen,
            CategoryNames = _context.BudgetCategories.Where(c => c.ProjectId == project.Id).Select(c => c.Name).ToList(),
            Categories = _context.BudgetCategories.Where(c => c.ProjectId == project.Id).Select(c => new BudgetCategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                BudgetAllocated = c.TotalAllocated.Amount,
                SpentAmount = c.Spent.Amount
            }).ToList()
        };

        return View(model);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectEditViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await _context.BudgetCategories.Where(c => c.ProjectId == model.Id)
                .Select(c => new BudgetCategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.Color,
                    BudgetAllocated = c.TotalAllocated.Amount,
                    SpentAmount = c.Spent.Amount
                }).ToListAsync();
            return View(model);
        }

        var project = await _projectRepo.GetByIdAsync(model.Id);
        if (project == null)
        {
            return NotFound();
        }

        project.Name = model.Name;
        project.StartDate = model.StartDate;
        project.EndDate = model.EndDate;
        project.Budget = new Money(model.Budget, "VND");
        project.Status = (ProjectStatus)model.Status;
        project.PublicEnabled = model.PublicEnabled;
        project.SubmissionsOpen = model.SubmissionsOpen;

        await _projectRepo.UpdateAsync(project);

        TempData["Success"] = "Project updated successfully.";
        return RedirectToAction("Index");
    }
}