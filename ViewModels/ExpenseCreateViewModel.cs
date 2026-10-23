using Microsoft.AspNetCore.Mvc.Rendering;
using Wigo.Models.Entities;
using Wigo.Models.ValueObjects;
using Wigo.Models.Enums;

namespace Wigo.ViewModels;

public class ExpenseCreateViewModel
{
    public int ProjectId { get; set; }
    public int? CategoryId { get; set; }
    public List<SelectListItem> Projects { get; set; } = new();
    public List<SelectListItem>? Categories { get; set; }
    public Money Amount { get; set; } = new();
}