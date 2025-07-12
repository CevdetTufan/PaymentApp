using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentApp.Api.Middlewares;

public class ExceptionMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionMiddleware> _logger;
	private readonly IHostEnvironment _hostEnvironment;

	private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment hostEnvironment)
	{
		_next = next;
		_logger = logger;
		_hostEnvironment = hostEnvironment;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception ex)
	{
		_logger.LogError(ex, "Exception");

		context.Response.ContentType = "application/json";
		context.Response.StatusCode = ex switch
		{
			KeyNotFoundException => StatusCodes.Status404NotFound,
			ArgumentException => StatusCodes.Status400BadRequest,
			UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
			_ => StatusCodes.Status500InternalServerError
		};

		string userMessage = ex switch
		{
			KeyNotFoundException => "The requested resource was not found.",
			ArgumentException => "Invalid request parameters.",
			UnauthorizedAccessException => "You are not authorized to access this resource.",
			_ => "An unexpected error occurred."
		};

		bool isDev = _hostEnvironment.IsDevelopment();

		var response = new
		{
			StatusCode = context.Response.StatusCode,
			Message = userMessage,
			Details = isDev ? ex.StackTrace : null
		};

		var json = JsonSerializer.Serialize(response, _jsonSerializerOptions);

		await context.Response.WriteAsync(json);
	}
}
