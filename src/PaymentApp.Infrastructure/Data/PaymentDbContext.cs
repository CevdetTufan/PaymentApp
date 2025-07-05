using Microsoft.EntityFrameworkCore;

namespace PaymentApp.Infrastructure.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }
}
