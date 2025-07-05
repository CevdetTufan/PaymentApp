using PaymentApp.Domain.ValueObjects;

namespace PaymentApp.Test.Domain;

public class MoneyTests
{
	[Fact]
	public void Ctor_NegativeAmount_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() => new Money(-1m, "USD"));
	}

	[Fact]
	public void Ctor_NullCurrency_ThrowsArgumentNullException()
	{
		Assert.Throws<ArgumentNullException>(() => new Money(10m, null!));
	}

	[Fact]
	public void Equals_SameValueObjects_AreEqual()
	{
		var m1 = new Money(100m, "EUR");
		var m2 = new Money(100m, "EUR");
		Assert.Equal(m1, m2);
		Assert.True(m1.Equals(m2));
	}

	[Fact]
	public void OperatorEquality_BothNull_ReturnsTrue()
	{
		Money? m1 = null;
		Money? m2 = null;

		Assert.True(m1 == m2);
		Assert.False(m1 != m2);
	}

	[Fact]
	public void OperatorEquality_LeftNullRightNotNull_ReturnsFalse()
	{
		Money? m1 = null;
		var m2 = new Money(10m, "USD");

		Assert.False(m1 == m2);
		Assert.True(m1 != m2);
	}

	[Fact]
	public void OperatorEquality_RightNullLeftNotNull_ReturnsFalse()
	{
		var m1 = new Money(10m, "USD");
		Money? m2 = null;

		Assert.False(m1 == m2);
		Assert.True(m1 != m2);
	}

	[Fact]
	public void OperatorEquality_SameValues_ReturnsTrue()
	{
		var m1 = new Money(50m, "EUR");
		var m2 = new Money(50m, "EUR");

		Assert.True(m1 == m2);
		Assert.False(m1 != m2);
	}

	[Fact]
	public void OperatorEquality_DifferentAmounts_ReturnsFalse()
	{
		var m1 = new Money(50m, "EUR");
		var m2 = new Money(60m, "EUR");

		Assert.False(m1 == m2);
		Assert.True(m1 != m2);
	}

	[Fact]
	public void OperatorEquality_DifferentCurrency_ReturnsFalse()
	{
		var m1 = new Money(50m, "EUR");
		var m2 = new Money(50m, "USD");

		Assert.False(m1 == m2);
		Assert.True(m1 != m2);
	}

	[Fact]
	public void OperatorEquality_SameReference_ReturnsTrue()
	{
		var m1 = new Money(10m, "GBP");
		var m2 = m1;

		Assert.True(m1 == m2);
		Assert.False(m1 != m2);
	}
}
