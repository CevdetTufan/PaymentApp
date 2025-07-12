using AutoMapper;
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
	private readonly IMapper _mapper;

	public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IMapper mapper)
	{
		_paymentRepository = paymentRepository;
		_mapper = mapper;
	}

	public async Task<PaymentDto> HandleAsync(CreatePaymentCommand command, CancellationToken token = default)
	{
		var dto = command.Payload;

		var payment = new Payment(
			dto.CustomerId,
			new Money(dto.Amount, dto.Currency)
		);

		await _paymentRepository.AddAsync(payment, token);

		return _mapper.Map<PaymentDto>(payment);
	}
}
