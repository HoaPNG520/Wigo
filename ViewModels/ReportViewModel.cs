namespace Wigo.ViewModels;

public class ReportViewModel
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = "";
    public DateTime GeneratedAt { get; set; }
    public List<ExpenseSummaryViewModel> Expenses { get; set; } = new();
}