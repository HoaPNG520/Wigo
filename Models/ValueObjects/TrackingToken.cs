using System.Security.Cryptography;
using System.Text;

namespace Wigo.Models.ValueObjects;

public class TrackingToken
{
    public int Id { get; set; }
    public string Value { get; set; } = "";
    public string Hash { get; set; } = "";
    public DateTime Expiry { get; set; }

    public TrackingToken()
    {
        Value = GenerateValue();
        Hash = HashValue(Value);
        Expiry = DateTime.UtcNow.AddDays(30);
    }

    private static string GenerateValue()
    {
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[24];
        rng.GetBytes(bytes);
        return "tk_" + Convert.ToBase64String(bytes);
    }

    public static string HashValue(string value)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public string ComputeHash() => HashValue(Value);

    public bool IsExpired() => DateTime.UtcNow > Expiry;
}