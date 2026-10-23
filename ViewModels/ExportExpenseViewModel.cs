using Wigo.Models.Enums;
using Wigo.Models.ValueObjects;

namespace Wigo.ViewModels;

public class ExportExpenseViewModel
{
    public int Id { get; set; }
    public string Amount { get; set; } = "";
    public ExpenseStatus Status { get; set; }
    public string SubmittedBy { get; set; } = "";
    public DateTime SubmittedAt { get; set; }
}