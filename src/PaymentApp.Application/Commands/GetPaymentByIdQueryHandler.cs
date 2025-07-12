using AutoMapper;
using PaymentApp.Application.Dtos;
using PaymentApp.Application.Interfaces.Handlers;
using PaymentApp.Application.Interfaces.Repositories;

namespace PaymentApp.Application.Commands;

public record GetPaymentByIdQuery(Guid Id)
	: IQuery<PaymentDto?>;

public class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, PaymentDto?>
{
	private readonly IPaymentRepository _paymentRepository;
	private readonly IMapper _mapper;

	public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
	{
		_paymentRepository = paymentRepository;
		_mapper = mapper;
	}

	public async Task<PaymentDto?> HandleAsync(GetPaymentByIdQuery query, CancellationToken token = default)
	{
		var payment = await _paymentRepository.GetByIdAsync(query.Id, token);

		if (payment is null) 
			throw new KeyNotFoundException($"Payment with ID {query.Id} not found.");

		return _mapper.Map<PaymentDto>(payment);
	}
}

