using PaymentApp.Infrastructure.Data;

namespace PaymentApp.Api.Endpoints;

public static class HealthEndpoints
{
	public static void MapHealthEndpoints(this IEndpointRouteBuilder app)
	{
		var health = app.MapGroup("/healthcheck")
						.WithTags("Health");

		health.MapGet("/", () =>
			Results.Ok($"Healthy. Request time is {DateTime.UtcNow}")
		)
		.WithName("HealthCheck")           
		.WithDescription("Health check")
		.WithOpenApi();                     

		health.MapGet("/db", async (PaymentDbContext db) =>
			await db.Database.CanConnectAsync()
				? Results.Ok("DB OK")
				: Results.StatusCode(503)
		)
		.WithName("DatabaseHealthCheck")
		.WithDescription("DB health check")
		.WithOpenApi();
	}
}
