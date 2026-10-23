using Wigo.Models.Entities;

namespace Wigo.ViewModels;

public class TrackingBillsViewModel
{
    public string Token { get; set; } = "";
    public List<Expense> Expenses { get; set; } = new();
}