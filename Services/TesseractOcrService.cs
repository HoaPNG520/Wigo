using System.Diagnostics;
using Wigo.Models.Entities;

namespace Wigo.Services;

public class TesseractOcrService : IOcrService
{
    public async Task<OcrResult> ExtractTextAsync(string imagePath, string language = "vie")
    {
        var result = new OcrResult { Provider = "Tesseract", ProcessedAt = DateTime.UtcNow };

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "tesseract",
                Arguments = $"\"{imagePath}\" stdout -l {language} --psm 6",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                return result;
            }

            result.RawText = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                result.Confidence = 0.0;
                return result;
            }

            result = ParseOcrOutput(result);
        }
        catch
        {
            // Return empty result on error, never throws
        }

        return result;
    }

    private OcrResult ParseOcrOutput(OcrResult result)
    {
        var fields = new Dictionary<string, string>();

        // Parse date (DD/MM/YYYY or similar patterns)
        var dateMatch = System.Text.RegularExpressions.Regex.Match(result.RawText, @"(\d{1,2}[\/\-]\d{1,2}[\/\-]\d{4})");
        if (dateMatch.Success)
        {
            fields["Date"] = dateMatch.Groups[1].Value;
        }

        // Parse total amount
        var totalMatch = System.Text.RegularExpressions.Regex.Match(result.RawText, @"total[^\n]{0,30}(\d[\d.,]*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (totalMatch.Success)
        {
            fields["Total"] = totalMatch.Groups[1].Value;
        }

        // Parse tax amount
        var taxMatch = System.Text.RegularExpressions.Regex.Match(result.RawText, @"thue[^\n]{0,30}(\d[\d.,]*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (taxMatch.Success)
        {
            fields["Tax"] = taxMatch.Groups[1].Value;
        }

        // Parse vendor name (first non-empty line that looks like a name)
        var lines = result.RawText.Split('\n');
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed) && !trimmed.Any(char.IsDigit) && trimmed.Length > 3)
            {
                fields["Vendor"] = trimmed;
                break;
            }
        }

        result.ExtractedFields = fields;

        // Calculate simple confidence based on text length and pattern matches
        if (!string.IsNullOrEmpty(result.RawText) && result.RawText.Length > 10)
        {
            result.Confidence = 0.75;
        }
        else
        {
            result.Confidence = 0.0;
        }

        return result;
    }
}