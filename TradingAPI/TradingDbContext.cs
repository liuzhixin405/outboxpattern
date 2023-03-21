using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TradingAPI
{
    public class TradingDbContext : DbContext
    {
        public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OutBoxMessage> OutBoxes { get; set; }
    }

    public class Order:IEntity
    {
        private Order() { }
        public Order(int customerId,int number ,decimal price,OrderStatus status)
        {
            EventId = Guid.NewGuid();
            CustomerId = customerId;
            Number = number;
            Price = price;
            Status = status;
            AddDomainEvent(new CreateOrderEvent(EventId, customerId));
        }
        [Key]
        public override int Id { get; protected set; }
        public Guid EventId { get; private set; }
        //public Guid Guid { get; private set; }=Guid.NewGuid();
        public int CustomerId { get; private set; }
        public int Number { get; private set; }
        public decimal Price { get; private set; }
        public OrderStatus Status { get; private set; }
    }

    public enum OrderStatus
    {
        Submitted,
        AwaitingValidation,
        StockConfirmed,
        Paid,
        Shipped,
        Cancelled
    }
}
