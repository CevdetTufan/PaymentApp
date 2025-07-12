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

			Results.Ok(dto);
		})
		.WithName("GetPaymentById")
		.WithOpenApi();


		// POST /CreatePayment
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


		// PUT /CompletePayment
		group.MapPut("/complete{paymentId:guid}", async (
				Guid paymentId,
				ICommandHandler<CompletePaymentCommand, PaymentDto> handler
			) =>
		{
			var result = await handler.HandleAsync(new CompletePaymentCommand(paymentId));
			return Results.Ok(result);
		})
		  .WithName("CompletePayment")
		  .WithOpenApi();
	}
}
