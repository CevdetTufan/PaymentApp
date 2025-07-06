using Microsoft.EntityFrameworkCore;
using PaymentApp.Application.Interfaces.Repositories;
using PaymentApp.Infrastructure.Data;

namespace PaymentApp.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
	protected readonly PaymentDbContext _context;
	protected readonly DbSet<T> _set;

	public Repository(PaymentDbContext context)
	{
		_context = context;
		_set = _context.Set<T>();
	}

	public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken token = default)
	{
		return await _set.FindAsync([id], token);
	}

	public virtual async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken token = default)
	{
		return await _set.ToListAsync(token);
	}

	public virtual async Task AddAsync(T entity, CancellationToken token = default)
	{
		await _set.AddAsync(entity, token);
		await _context.SaveChangesAsync(token);
	}

	public virtual async Task UpdateAsync(T entity, CancellationToken token = default)
	{
		_set.Update(entity);
		await _context.SaveChangesAsync(token);
	}

	public virtual async Task DeleteAsync(T entity, CancellationToken token = default)
	{
		_set.Remove(entity);
		await _context.SaveChangesAsync(token);
	}
}
