using Microsoft.AspNetCore.Mvc.Rendering;
using Wigo.Models.Entities;
using Wigo.Models.Enums;

namespace Wigo.ViewModels;

public class ProjectEditViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Status { get; set; }
    public decimal Budget { get; set; }
    public bool PublicEnabled { get; set; }
    public bool SubmissionsOpen { get; set; }
    public List<string> CategoryNames { get; set; } = new();
    public List<BudgetCategoryViewModel> Categories { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}