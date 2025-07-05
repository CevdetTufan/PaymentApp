using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PaymentApp.Application.Modules;
using PaymentApp.Infrastructure.Data;
using PaymentApp.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
	.AddDbContext<PaymentDbContext>(options =>
		options.UseNpgsql(connStr));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
			.ConfigureContainer<ContainerBuilder>(container =>
			{
				container.RegisterModule(new DataModule());
				container.RegisterModule(new ServiceModule());
			});

builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/healtcheck", () =>
{
    return Results.Ok($"Healthy. Request time is {DateTime.UtcNow}");
})
.WithName("healtcheck");


app.MapGet("/healthcheck-db", async (PaymentDbContext db) =>
{
	return await db.Database.CanConnectAsync()
		? Results.Ok("DB OK")
		: Results.StatusCode(503);
});


await app.RunAsync();