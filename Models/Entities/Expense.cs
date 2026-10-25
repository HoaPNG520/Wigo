namespace Wigo.Models.Entities;

using Wigo.Models.Enums;
using Wigo.Models.ValueObjects;

public class Expense
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int? CategoryId { get; set; }
    public int? TrackingTokenId { get; set; }
    public Money Amount { get; set; } = new();
    public ExpenseStatus Status { get; set; }
    public string SubmittedBy { get; set; } = "";
    public DateTime SubmittedAt { get; set; }
    public int? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }
    public double? AiConfidence { get; set; }
    public string AiExtractedFields { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Title { get; set; } = "";
    public string Payer { get; set; } = "";
    public string Time { get; set; } = "";
    public string Date { get; set; } = "";
    public string FundingSource { get; set; } = "";
    public string Vendor { get; set; } = "";
    public string InvoiceNumber { get; set; } = "";
    public RiskLevel Risk { get; set; } = RiskLevel.None;
    public bool IsFresh { get; set; }
    public virtual List<ExpenseItem> Items { get; set; } = new();

    public virtual Project Project { get; set; } = null!;
    public virtual BudgetCategory? Category { get; set; }
    public virtual TrackingToken? TrackingToken { get; set; }
    public virtual List<EvidenceFile> EvidenceFiles { get; set; } = new();
}