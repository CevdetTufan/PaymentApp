using PaymentApp.Domain.Enums;
using PaymentApp.Domain.Exceptions;
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
		if (customerId == Guid.Empty)
		{
			throw new BusinessRuleViolationException("Customer ID cannot be empty.");
		}

		if (amount == null)
		{
			throw new BusinessRuleViolationException("Payment amount cannot be null.");
		}

		if (amount.Amount <= 0)
		{
			throw new BusinessRuleViolationException("Payment amount must be greater than zero.");
		}

		Id = Guid.NewGuid();
		CustomerId = customerId;
		Amount = amount;
		Status = PaymentStatus.Pending;
		CreatedAt = DateTime.UtcNow;
	}

	public void MarkCompleted()
	{
		if (Status != PaymentStatus.Pending)
		{
			throw new BusinessRuleViolationException("Payment can only be marked as completed if it is pending.");
		}

		if (Amount.Amount <= 0)
		{
			throw new BusinessRuleViolationException("Payment amount must be greater than zero to complete the payment.");
		}


		Status = PaymentStatus.Completed;
		ProcessedAt = DateTime.UtcNow;
	}

	public void MarkFailed()
	{
		if(Status != PaymentStatus.Pending)
		{
			throw new BusinessRuleViolationException("Payment can only be marked as failed if it is pending.");
		}

		Status = PaymentStatus.Failed;
		ProcessedAt = DateTime.UtcNow;
	}

	public void MarkRefunded()
	{
		if (Status != PaymentStatus.Completed)
		{
			throw new BusinessRuleViolationException("Payment can only be refunded if it is completed.");
		}

		if (ProcessedAt.HasValue && ProcessedAt.Value.AddDays(30) < DateTime.UtcNow)
		{
			throw new BusinessRuleViolationException("Payment can only be refunded within 30 days of processing.");
		}

		Status = PaymentStatus.Refunded;
		ProcessedAt = DateTime.UtcNow;
	}
}
