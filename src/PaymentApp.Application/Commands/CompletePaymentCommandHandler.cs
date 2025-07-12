using AutoMapper;
using PaymentApp.Application.Dtos;
using PaymentApp.Application.Interfaces.Handlers;
using PaymentApp.Application.Interfaces.Repositories;

namespace PaymentApp.Application.Commands;

public record CompletePaymentCommand(Guid PaymentId) : ICommand<PaymentDto>;

public class CompletePaymentCommandHandler : ICommandHandler<CompletePaymentCommand, PaymentDto>
{
	private readonly IPaymentRepository _paymentRepository;
	private readonly IMapper _mapper;

	public CompletePaymentCommandHandler(IPaymentRepository paymentRepository, IMapper mapper)
	{
		_paymentRepository = paymentRepository;
		_mapper = mapper;
	}
	public async Task<PaymentDto> HandleAsync(CompletePaymentCommand command, CancellationToken token = default)
	{
		var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, token) 
					?? throw new KeyNotFoundException($"Payment with ID {command.PaymentId} not found.");

		payment.MarkCompleted();

		await _paymentRepository.UpdateAsync(payment, token);

		return _mapper.Map<PaymentDto>(payment);
	}
}
