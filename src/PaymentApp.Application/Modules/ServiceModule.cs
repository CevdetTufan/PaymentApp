using Autofac;
using PaymentApp.Application.Commands;
using PaymentApp.Application.Dtos;
using PaymentApp.Application.Interfaces.Handlers;

namespace PaymentApp.Application.Modules;

public class ServiceModule : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		// Komut handler’ları
		builder.RegisterType<CreatePaymentCommandHandler>()
			   .As<ICommandHandler<CreatePaymentCommand, PaymentDto>>()
			   .InstancePerLifetimeScope();

		// Sorgu handler’ları
		builder.RegisterType<GetPaymentByIdQueryHandler>()
			   .As<IQueryHandler<GetPaymentByIdQuery, PaymentDto>>()
			   .InstancePerLifetimeScope();
	}
}
