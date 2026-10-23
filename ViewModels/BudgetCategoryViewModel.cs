using Microsoft.AspNetCore.Mvc.Rendering;

namespace Wigo.ViewModels;

public class BudgetCategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Color { get; set; } = "";
    public decimal BudgetAllocated { get; set; }
    public decimal SpentAmount { get; set; }
}