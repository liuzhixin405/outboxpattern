using BillAPI.Events;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.EventBus.AspNetCore.Attributes;
using RabbitMQ.EventBus.AspNetCore.Events;
using System.Threading;

namespace BillAPI
{
    [EventBus(Exchange = "RabbitMQ.EventBus.Simple", RoutingKey = "rabbitmq.eventbus.test")]
    public class IDomainEvent : IEvent
    {
        public Guid Id { get; set; }  
        public string Content { get; set; }
    }
    public  class IdempotentDomainEventHandler : IEventResponseHandler<IDomainEvent,int>,IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        public IdempotentDomainEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBodyHandle Disposable.");
        }

        public async Task<int> HandleAsync(HandlerEventArgs<IDomainEvent> args)
        {
            using var scope = _serviceProvider.CreateScope();
            BillingDbContext _context = scope.ServiceProvider.GetService<BillingDbContext>();
            string consumer = args.GetType().Name;
            if (await _context.Set<OutboxMessageConsumer>().AnyAsync(o => o.Guid == args.EventObject.Id && o.Name==consumer))
            {
                return default;
            }
            Console.WriteLine($"等待处理的消息{args.EventObject.Content}");
            CreateOrderEvent createOrderEvent = System.Text.Json.JsonSerializer.Deserialize<CreateOrderEvent>(args.EventObject.Content);
            await _context.BillingRecords.AddAsync(new BillingRecord { CreateTime=DateTime.UtcNow, OrderEventId=createOrderEvent.EventId});
            Console.WriteLine($"处理的消息完毕");

            _context.Set<OutboxMessageConsumer>().Add(new OutboxMessageConsumer
            {
                Guid = args.EventObject.Id,
                Name = consumer
            });
           return await _context.SaveChangesAsync();
        }
    }

    public class OutboxMessageConsumer
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; }
    }
}
