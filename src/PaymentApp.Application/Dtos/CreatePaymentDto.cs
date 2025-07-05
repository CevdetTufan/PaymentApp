namespace PaymentApp.Application.Dtos;

public record CreatePaymentDto(
	  Guid CustomerId,
	  decimal Amount,
	  string Currency
  );
