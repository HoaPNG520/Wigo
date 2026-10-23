using Wigo.Models.Enums;
using Wigo.Models.ValueObjects;

namespace Wigo.ViewModels;

public class ExpenseSummaryViewModel
{
    public int Id { get; set; }
    public Money Amount { get; set; } = new();
    public ExpenseStatus Status { get; set; }
    public string SubmittedBy { get; set; } = "";
}