using RabbitMQ.EventBus.AspNetCore.Attributes;
using RabbitMQ.EventBus.AspNetCore.Events;

namespace BillAPI.Events
{
    //[EventBus(Exchange = "RabbitMQ.EventBus.Simple", RoutingKey = "rabbitmq.eventbus.test")]
    //public class MessageBody:IEvent
    //{
    //    public string Body { get; set; }
    //    public DateTimeOffset Time { get; set; }
    //}
}
