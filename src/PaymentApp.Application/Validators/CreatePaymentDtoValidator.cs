using FluentValidation;
using PaymentApp.Application.Dtos;

namespace PaymentApp.Application.Validators;

public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
{
	public CreatePaymentDtoValidator()
	{
		RuleFor(x => x.CustomerId)
			.NotEmpty().WithMessage("Customer ID is required.")
			.Must(BeAValidGuid).WithMessage("Customer ID must be a valid GUID.");
		RuleFor(x => x.Amount)
			.GreaterThan(0).WithMessage("Amount must be greater than zero.");
		RuleFor(x => x.Currency)
			.NotEmpty().WithMessage("Currency is required.")
			.Length(3).WithMessage("Currency must be a 3-letter code.");
	}

	private static bool BeAValidGuid(Guid id) => id != Guid.Empty;
}
