using Microsoft.EntityFrameworkCore;
using PaymentApp.Domain.Entities;

namespace PaymentApp.Infrastructure.Data;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
{
	public DbSet<Payment> Payments { get; set; }
}
