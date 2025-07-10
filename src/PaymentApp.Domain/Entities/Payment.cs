using PaymentApp.Domain.Enums;
using PaymentApp.Domain.ValueObjects;

namespace PaymentApp.Domain.Entities;

public class Payment
{
	public Guid Id { get; private set; }
	public Guid CustomerId { get; private set; }
	public Money Amount { get; private set; }
	public PaymentStatus Status { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime? ProcessedAt { get; private set; }

	protected Payment()
	{
		// EF Core requires a parameterless constructor for entity instantiation
	}

	public Payment(Guid customerId, Money amount)
	{
		Id = Guid.NewGuid();
		CustomerId = customerId;
		Amount = amount;
		Status = PaymentStatus.Pending;
		CreatedAt = DateTime.UtcNow;
	}

	public void MarkCompleted()
	{
		Status = PaymentStatus.Completed;
		ProcessedAt = DateTime.UtcNow;
	}

	public void MarkFailed()
	{
		Status = PaymentStatus.Failed;
		ProcessedAt = DateTime.UtcNow;
	}

	public void MarkRefunded()
	{
		Status = PaymentStatus.Refunded;
		ProcessedAt = DateTime.UtcNow;
	}
}
