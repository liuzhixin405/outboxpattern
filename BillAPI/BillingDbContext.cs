using Microsoft.EntityFrameworkCore;

namespace BillAPI
{
    public class BillingDbContext:DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options):base(options)
        {
            
        }
        public DbSet<BillingRecord> BillingRecords { get; set; }

        public DbSet<OutboxMessageConsumer> OutboxMessages { get; set; } 
    }
}
