using Wigo.Models.Entities;

namespace Wigo.ViewModels;

public class TrackingResubmitViewModel
{
    public string Token { get; set; } = "";
    public int ExpenseId { get; set; }
    public Expense Expense { get; set; } = new();
}