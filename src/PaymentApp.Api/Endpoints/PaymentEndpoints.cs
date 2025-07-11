using PaymentApp.Api.Filters;
using PaymentApp.Application.Commands;
using PaymentApp.Application.Dtos;
using PaymentApp.Application.Interfaces.Handlers;

namespace PaymentApp.Api.Endpoints;

public static class PaymentEndpoints
{
	public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/payments")
					   .WithTags("Payments");


		// GET /payments/{id}
		group.MapGet("/{id:guid}", async (
				Guid id,
				IQueryHandler<GetPaymentByIdQuery, PaymentDto?> handler
			) =>
		{
			var dto = await handler.HandleAsync(new GetPaymentByIdQuery(id));

			return dto is null
				? Results.NotFound()
				: Results.Ok(dto);
		})
		.WithName("GetPaymentById")
		.WithOpenApi();


		// POST /payments
		group.MapPost("", async (
				 CreatePaymentDto dto,
				 ICommandHandler<CreatePaymentCommand, PaymentDto> handler
			 ) =>
		{
			var result = await handler.HandleAsync(new CreatePaymentCommand(dto));
			return Results.Created($"/payments/{result.Id}", result);
		})
		 .AddEndpointFilter<ValidationFilter<CreatePaymentDto>>()
		 .WithName("CreatePayment")
		 .WithOpenApi();
	}
}
