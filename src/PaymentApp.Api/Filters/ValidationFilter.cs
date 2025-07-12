using FluentValidation;

namespace PaymentApp.Api.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
	private readonly IValidator<T> _validator;
	public ValidationFilter(IValidator<T> validator) => _validator = validator;

	public async ValueTask<object?> InvokeAsync(
		EndpointFilterInvocationContext context,
		EndpointFilterDelegate next)
	{
		var dto = context.Arguments.OfType<T>().FirstOrDefault();
		if (dto != null)
		{
			var result = await _validator.ValidateAsync(dto);
			if (!result.IsValid)
			{
				var errors = result.Errors
					.Select(e => new { e.PropertyName, e.ErrorMessage });
				return Results.BadRequest(errors);
			}
		}

		return await next(context);
	}
}
