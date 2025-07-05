using PaymentApp.Application.Dtos;
using PaymentApp.Application.Interfaces.Handlers;
using PaymentApp.Application.Interfaces.Repositories;
using PaymentApp.Domain.Entities;
using PaymentApp.Domain.ValueObjects;

namespace PaymentApp.Application.Commands;

public record CreatePaymentCommand(CreatePaymentDto Payload) 
	: ICommand<PaymentDto>;

public class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, PaymentDto>
{
	private readonly IPaymentRepository _paymentRepository;

	public CreatePaymentCommandHandler(IPaymentRepository paymentRepository)
	{
		_paymentRepository = paymentRepository;
	}

	public async Task<PaymentDto> HandleAsync(CreatePaymentCommand command, CancellationToken token = default)
	{
		var dto = command.Payload;

		var payment = new Payment(
			dto.CustomerId,
			new Money(dto.Amount, dto.Currency)
		);

		await _paymentRepository.AddAsync(payment, token);


		return new PaymentDto(
			payment.Id,
			payment.CustomerId,
			payment.Amount.Amount,
			payment.Amount.Currency,
			payment.Status.ToString(),
			payment.CreatedAt,
			payment.ProcessedAt
		);
	}
}
