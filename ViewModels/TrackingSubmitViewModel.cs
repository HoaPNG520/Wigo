using Wigo.Models.Entities;

namespace Wigo.ViewModels;

public class TrackingSubmitViewModel
{
    public string Token { get; set; } = "";
    public Expense Expense { get; set; } = new();
}