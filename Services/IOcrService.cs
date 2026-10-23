namespace Wigo.Services;

public class OcrResult
{
    public string RawText { get; set; } = "";
    public Dictionary<string, string> ExtractedFields { get; set; } = new();
    public double Confidence { get; set; }
    public string Provider { get; set; } = "";
    public DateTime ProcessedAt { get; set; }
}

public interface IOcrService
{
    Task<OcrResult> ExtractTextAsync(string imagePath, string language = "vie");
}