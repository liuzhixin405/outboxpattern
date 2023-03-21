using RabbitMQ.EventBus.AspNetCore.Events;

namespace TradingAPI
{
    public class CreateOrderEvent:IEvent
    {
        public Guid EventId { get; set; }
        public int CustomerId { get; set; }
        public CreateOrderEvent(Guid EventId,int customerId)
        {
            this.EventId = EventId;
            CustomerId = customerId;
        }

    }
}
