using Microsoft.AspNetCore.Mvc.Rendering;
using Wigo.Models.Enums;
using Wigo.Models.Entities;

namespace Wigo.ViewModels;

public class ExpenseIndexViewModel
{
    public List<Expense> Expenses { get; set; } = new();
    public string? StatusFilter { get; set; }
    public List<string> StatusFilters { get; set; } = new();
    public List<SelectListItem> Projects { get; set; } = new();
    public int? ProjectFilter { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}