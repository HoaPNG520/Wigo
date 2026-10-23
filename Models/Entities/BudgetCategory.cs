namespace Wigo.Models.Entities;

using Wigo.Models.ValueObjects;

public class BudgetCategory
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Name { get; set; } = "";
    public Money TotalAllocated { get; set; } = new();
    public Money Spent { get; set; } = new();
    public string Color { get; set; } = "";
    public int SortOrder { get; set; }

    public virtual Project Project { get; set; } = null!;
}