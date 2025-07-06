using Microsoft.EntityFrameworkCore;

namespace PaymentApp.Domain.ValueObjects;

[Owned]
public sealed class Money : IEquatable<Money> // Sealed the class to address S4035
{
	public decimal Amount { get; }
	public string Currency { get; }

	private Money() { } // Private constructor for EF Core

	public Money(decimal amount, string currency)
	{
		if (amount < 0) throw new ArgumentException("Amount must be non-negative", nameof(amount));
		Amount = amount;
		Currency = currency ?? throw new ArgumentNullException(nameof(currency));
	}

	public override bool Equals(object? obj)
		=> Equals(obj as Money);

	public bool Equals(Money? other)
	{
		if (ReferenceEquals(other, null)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Amount == other.Amount
			&& Currency.Equals(other.Currency, StringComparison.Ordinal);
	}

	public override int GetHashCode()
		=> HashCode.Combine(Amount, Currency);

	public static bool operator ==(Money? a, Money? b)
		=> a?.Equals(b) ?? b is null;

	public static bool operator !=(Money? a, Money? b)
		=> !(a == b);
}
