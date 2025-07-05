using PaymentApp.Domain.Entities;
using PaymentApp.Domain.Enums;
using PaymentApp.Domain.ValueObjects;

namespace PaymentApp.Test.Domain;

public class PaymentEntityTests
{
	[Fact]
	public void Ctor_WithValidParameters_SetsPendingStatusAndTimestamps()
	{
		// Arrange
		var customerId = Guid.NewGuid();
		var amount = new Money(100m, "USD");

		// Act
		var payment = new Payment(customerId, amount);

		// Assert
		Assert.Equal(customerId, payment.CustomerId);
		Assert.Equal(amount, payment.Amount);
		Assert.Equal(PaymentStatus.Pending, payment.Status);
		Assert.True((DateTime.UtcNow - payment.CreatedAt).TotalSeconds < 5);
		Assert.Null(payment.ProcessedAt);
	}

	[Fact]
	public void MarkCompleted_SetsStatusToCompletedAndProcessedAt()
	{
		// Arrange
		var payment = new Payment(Guid.NewGuid(), new Money(10m, "EUR"));

		// Act
		payment.MarkCompleted();

		// Assert
		Assert.Equal(PaymentStatus.Completed, payment.Status);
		Assert.NotNull(payment.ProcessedAt);
		Assert.True((DateTime.UtcNow - payment.ProcessedAt.Value).TotalSeconds < 5);
	}

	[Fact]
	public void MarkFailed_SetsStatusToFailedAndProcessedAt()
	{
		// Arrange
		var payment = new Payment(Guid.NewGuid(), new Money(5m, "GBP"));

		// Act
		payment.MarkFailed();

		// Assert
		Assert.Equal(PaymentStatus.Failed, payment.Status);
		Assert.NotNull(payment.ProcessedAt);
	}

	[Fact]
	public void MarkRefunded_SetsStatusToRefundedAndProcessedAt()
	{
		// Arrange
		var payment = new Payment(Guid.NewGuid(), new Money(20m, "JPY"));

		// Act
		payment.MarkRefunded();

		// Assert
		Assert.Equal(PaymentStatus.Refunded, payment.Status);
		Assert.NotNull(payment.ProcessedAt);
	}

	[Fact]
	public void MultipleStatusChanges_ReflectLastChangeOnly()
	{
		// Arrange
		var payment = new Payment(Guid.NewGuid(), new Money(1m, "USD"));

		// Act
		payment.MarkCompleted();
		var firstProcessed = payment.ProcessedAt;
		payment.MarkFailed();
		var secondProcessed = payment.ProcessedAt;

		// Assert
		Assert.Equal(PaymentStatus.Failed, payment.Status);
		Assert.NotEqual(firstProcessed, secondProcessed);
	}
}