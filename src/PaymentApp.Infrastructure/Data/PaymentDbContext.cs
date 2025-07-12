using Microsoft.EntityFrameworkCore;
using PaymentApp.Domain.Entities;

namespace PaymentApp.Infrastructure.Data;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
{
	public DbSet<Payment> Payments { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Payment>(eb =>
		{
			eb.OwnsOne(p => p.Amount, amt =>
			{
				amt.Property(m => m.Amount)
					.HasColumnName("Amount")    
					.IsRequired();
				amt.Property(m => m.Currency)
					.HasColumnName("Currency")   
					.HasMaxLength(3)
					.IsRequired();
			});
		});
	}
}
