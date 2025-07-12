using Autofac;
using AutoMapper;
using PaymentApp.Application.Commands;
using PaymentApp.Application.Dtos;
using PaymentApp.Application.Interfaces.Handlers;
using PaymentApp.Application.Interfaces.Repositories;
using PaymentApp.Application.Mappings;

namespace PaymentApp.Application.Modules;

public class ServiceModule : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterAssemblyTypes(typeof(PaymentProfile).Assembly)
			 .AssignableTo<Profile>()
			 .As<Profile>()
			 .SingleInstance();

		builder.Register(ctx =>
		{
			var profiles = ctx.Resolve<IEnumerable<Profile>>();
			var config = new MapperConfiguration(cfg =>
			{
				foreach (var profile in profiles)
					cfg.AddProfile(profile);
			});
			return config;
		})
		.AsSelf()
		.SingleInstance();

		builder.Register(ctx =>
		{
			var c = ctx.Resolve<MapperConfiguration>();
			return c.CreateMapper(ctx.Resolve);
		})
		.As<IMapper>()
		.InstancePerLifetimeScope();


		// Komut handler’ları
		builder.RegisterType<CreatePaymentCommandHandler>()
			   .As<ICommandHandler<CreatePaymentCommand, PaymentDto>>()
			   .InstancePerLifetimeScope();

		builder.RegisterType<CompletePaymentCommandHandler>()
			.As<ICommandHandler<CompletePaymentCommand, PaymentDto>>()
			   .InstancePerLifetimeScope();

		// Sorgu handler’ları
		builder.RegisterType<GetPaymentByIdQueryHandler>()
			   .As<IQueryHandler<GetPaymentByIdQuery, PaymentDto?>>()
			   .InstancePerLifetimeScope();
	}
}
