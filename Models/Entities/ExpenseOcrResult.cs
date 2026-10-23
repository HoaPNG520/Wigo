namespace Wigo.Models.Entities;

public class ExpenseOcrResult
{
    public int Id { get; set; }
    public int ExpenseId { get; set; }
    public string RawText { get; set; } = "";
    public string ExtractedFields { get; set; } = "";
    public double ConfidenceScore { get; set; }
    public string Provider { get; set; } = "";
    public DateTime ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Expense Expense { get; set; } = null!;
}