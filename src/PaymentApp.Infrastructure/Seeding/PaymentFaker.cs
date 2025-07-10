using Bogus;
using PaymentApp.Domain.Entities;
using PaymentApp.Domain.Enums;
using PaymentApp.Domain.ValueObjects;

namespace PaymentApp.Infrastructure.Seeding;
public static class PaymentFaker
{
	private static readonly string[] currencyOptions = ["USD", "EUR", "GBP", "TRY"];

	public static List<Payment> Get(int count = 50)
	{
		IEnumerable<Guid> customerIds = Enumerable.Range(1, 10)
			.Select(i => Guid.NewGuid());

		var faker = new Faker<Payment>()
				.CustomInstantiator(f =>
				{
					var customerId = f.PickRandom(customerIds);
					var amount = new Money(
						decimal.Round(f.Random.Decimal(5, 2000), 2),
						f.PickRandom(currencyOptions));

					return new Payment(customerId, amount);
				})
				.RuleFor(p => p.CreatedAt, f =>
				{
					 var local = f.Date.Past(3);
					 return DateTime.SpecifyKind(local, DateTimeKind.Utc);
				})
				.RuleFor(p => p.Status, f => f.PickRandom<PaymentStatus>())
				.RuleFor(p => p.ProcessedAt, (f, p) =>
					p.Status == PaymentStatus.Pending
					  ? null
					  : f.Date.Between(p.CreatedAt, DateTime.UtcNow)
		);

		var payments = faker.Generate(count);
		return payments;
	}
}