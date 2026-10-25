namespace Wigo.Models.Entities;
using Wigo.Models.ValueObjects;

public class ExpenseItem
{
    public int Id { get; set; }
    public int ExpenseId { get; set; }
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public Money UnitPrice { get; set; } = new();
    public virtual Expense Expense { get; set; } = null!;
}