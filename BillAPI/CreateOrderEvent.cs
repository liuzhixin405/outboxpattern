using RabbitMQ.EventBus.AspNetCore.Events;

namespace BillAPI
{
   
    /// <summary>
    /// 来自tradingapi的数据
    /// </summary>
    public class CreateOrderEvent
    {
        public Guid EventId { get; set; }
        public int CustomerId { get; set; }
    }
}
