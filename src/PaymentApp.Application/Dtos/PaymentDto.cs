namespace PaymentApp.Application.Dtos;

public record PaymentDto(
	   Guid Id,
	   Guid CustomerId,
	   decimal Amount,
	   string Currency,
	   string Status,
	   DateTime CreatedAt,
	   DateTime? ProcessedAt
   );
