namespace PaymentApp.Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
	Task<T?> GetByIdAsync(Guid id, CancellationToken token = default);
	Task<IReadOnlyList<T>> ListAllAsync(CancellationToken token = default);
	Task AddAsync(T entity, CancellationToken token = default);
	Task UpdateAsync(T entity, CancellationToken token = default);
	Task DeleteAsync(T entity, CancellationToken token = default);
}
