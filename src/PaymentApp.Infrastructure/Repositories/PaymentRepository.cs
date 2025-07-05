using Microsoft.EntityFrameworkCore;
using PaymentApp.Application.Interfaces.Repositories;
using PaymentApp.Domain.Entities;
using PaymentApp.Infrastructure.Data;

namespace PaymentApp.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
	public PaymentRepository(PaymentDbContext context)
		: base(context)
	{
	}

	public async Task<IReadOnlyList<Payment>> ListByCustomerAsync(Guid customerId, CancellationToken token = default)
	{
		return await _context.Payments
			.Where(p => p.CustomerId == customerId)
			.ToListAsync(token);
	}
}
