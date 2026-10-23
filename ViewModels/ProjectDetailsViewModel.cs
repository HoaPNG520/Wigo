using Wigo.Models.Enums;
using Wigo.Models.ValueObjects;

namespace Wigo.ViewModels;

public class ProjectDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Status { get; set; }
    public string OrganizationName { get; set; } = "";
    public Money Budget { get; set; } = new();
    public Money BaseSpent { get; set; } = new();
    public bool PublicEnabled { get; set; }
    public bool SubmissionsOpen { get; set; }
    public List<CategoryBudgetViewModel> Categories { get; set; } = new();
    public List<ExpenseSummaryViewModel> Expenses { get; set; } = new();
}