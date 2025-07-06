using Autofac;
using PaymentApp.Application.Interfaces.Repositories;
using PaymentApp.Infrastructure.Data;
using PaymentApp.Infrastructure.Repositories;


namespace PaymentApp.Infrastructure.Modules;

public class DataModule : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		// DbContext
		builder.RegisterType<PaymentDbContext>()
			   .AsSelf()
			   .InstancePerLifetimeScope();

		// Generic repository
		builder.RegisterGeneric(typeof(Repository<>))
			   .As(typeof(IRepository<>))
			   .InstancePerLifetimeScope();

		// Payment repository
		builder.RegisterType<PaymentRepository>()
			   .As<IPaymentRepository>()
			   .InstancePerLifetimeScope();
	}
}
