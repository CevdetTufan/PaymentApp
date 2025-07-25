using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PaymentApp.Api.Endpoints;
using PaymentApp.Api.Filters;
using PaymentApp.Api.Middlewares;
using PaymentApp.Api.Utilities;
using PaymentApp.Application.Modules;
using PaymentApp.Application.Validators;
using PaymentApp.Infrastructure.Data;
using PaymentApp.Infrastructure.Modules;
using PaymentApp.Infrastructure.Seeding;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.ConfigureHttpJsonOptions(opts =>
{
	opts.SerializerOptions.Converters.Add(new ForgivingGuidConverter());
});

builder.Services
	.AddDbContext<PaymentDbContext>(options =>
		options.UseNpgsql(connStr));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
			.ConfigureContainer<ContainerBuilder>(container =>
			{
				container.RegisterModule(new DataModule());
				container.RegisterModule(new ServiceModule());

				container.RegisterAssemblyTypes(typeof(CreatePaymentDtoValidator).Assembly)
					.AsClosedTypesOf(typeof(IValidator<>))
					.AsImplementedInterfaces()               
					.InstancePerLifetimeScope();

				container.RegisterGeneric(typeof(ValidationFilter<>))
				  .AsSelf()                               
				  .InstancePerLifetimeScope();
			});

builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

	app.MapScalarApiReference(options =>
	{
		options
			.WithTitle("PaymentApp API")
			.WithDarkMode()   
			.WithTheme(ScalarTheme.Saturn);
	});
}

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
	db.Database.Migrate();

	if (app.Environment.IsDevelopment() && !await db.Payments.AnyAsync())
	{
		var payments = PaymentFaker.Get(1000);
		await db.Payments.AddRangeAsync(payments);
		await db.SaveChangesAsync();
	}
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.MapHealthEndpoints();
app.MapPaymentEndpoints();

await app.RunAsync();