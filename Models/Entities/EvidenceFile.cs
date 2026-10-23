namespace Wigo.Models.Entities;

public class EvidenceFile
{
    public int Id { get; set; }
    public int ExpenseId { get; set; }
    public string BlobUrl { get; set; } = "";
    public string FileName { get; set; } = "";
    public string MimeType { get; set; } = "";
    public long FileSize { get; set; }
    public int Version { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool IsMasked { get; set; }
    public string RawText { get; set; } = "";
    public string ExtractedFields { get; set; } = "";

    public virtual Expense Expense { get; set; } = null!;
}