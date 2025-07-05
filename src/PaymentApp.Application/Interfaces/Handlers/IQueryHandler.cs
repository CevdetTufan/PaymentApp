namespace PaymentApp.Application.Interfaces.Handlers;

public interface IQuery<TResult> { }

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
	Task<TResult?> HandleAsync(TQuery query, CancellationToken token = default);
}
