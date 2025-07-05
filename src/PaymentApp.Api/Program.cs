using Autofac;
using Autofac.Extensions.DependencyInjection;
using PaymentApp.Application.Modules;
using PaymentApp.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);

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

await app.RunAsync();