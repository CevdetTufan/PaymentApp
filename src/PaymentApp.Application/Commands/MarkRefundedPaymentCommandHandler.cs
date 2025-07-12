using AutoMapper;
using PaymentApp.Application.Dtos;
using PaymentApp.Application.Interfaces.Handlers;
using PaymentApp.Application.Interfaces.Repositories;

namespace PaymentApp.Application.Commands;

public record MarkRefundedPaymentCommand(Guid PaymentId) : ICommand<PaymentDto>;

public class MarkRefundedPaymentCommandHandler : ICommandHandler<MarkRefundedPaymentCommand, PaymentDto>
{
	private readonly IPaymentRepository _paymentRepository;
	private readonly IMapper _mapper;

	public MarkRefundedPaymentCommandHandler(IPaymentRepository paymentRepository, IMapper mapper)
	{
		_paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
	}
	public async Task<PaymentDto> HandleAsync(MarkRefundedPaymentCommand command, CancellationToken token = default)
	{
		var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, token)
					   ?? throw new KeyNotFoundException($"Payment with ID {command.PaymentId} not found.");

		payment.MarkRefunded();

		await _paymentRepository.UpdateAsync(payment, token);

		return _mapper.Map<PaymentDto>(payment);
	}
}
