namespace Wigo.Models.ValueObjects;

public class Money
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";

    public Money(decimal amount = 0, string currency = "VND")
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount, a.Currency);
    public static Money operator -(Money a, Money b) => new(a.Amount - b.Amount, a.Currency);
    public static bool operator ==(Money a, Money b) => a.Amount == b.Amount && a.Currency == b.Currency;
    public static bool operator !=(Money a, Money b) => !(a == b);

    public override bool Equals(object? obj)
    {
        if (obj is Money other)
            return Amount == other.Amount && Currency == other.Currency;
        return false;
    }

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public string Format() => Amount.ToString("#,##0", new System.Globalization.NumberFormatInfo { NumberGroupSeparator = "," }) + "₫";
}