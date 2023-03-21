using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.EventBus.AspNetCore;

namespace TradingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly TradingDbContext _context;
        private readonly IRabbitMQEventBus _eventBus;

        public OrderController(TradingDbContext context, IRabbitMQEventBus eventBus)
        {
            _context = context;
            _eventBus = eventBus;
        }
        [HttpGet("GetOrders")]
        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        [HttpPut]
        public async Task CreateOrder(CreateOrderDTO order)
        {
            if (order is null) return;
            var createOrder = new Order(order.CustomerId,order.Number,order.Price,order.Status);
            await _context.Orders.AddAsync(createOrder);
             await  _context.SaveChangesAsync();
        }
        /// <summary>
        /// 测试链接
        /// </summary>
        /// <returns></returns>
        [HttpGet("publish")]
        public async Task<ActionResult<string>> Get()
        {
            Console.WriteLine($"发送消息{1}");
            var body = new
            {
                requestId = Guid.NewGuid(),
                Body = $"rabbitmq.eventbus.test=>发送消息\t{1}",
                Time = DateTimeOffset.Now,
            };
            var r = await _eventBus.PublishAsync<string>(body, exchange: "RabbitMQ.EventBus.Simple", routingKey: "rabbitmq.eventbus.test");
            Console.WriteLine($"返回了{r}");
            await Task.Delay(500);
            return r;
        }
    }
}
