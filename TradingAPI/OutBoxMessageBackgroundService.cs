using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Plugins;
using Polly;
using RabbitMQ.EventBus.AspNetCore;
using System.Security.Policy;

namespace TradingAPI
{
    public class OutBoxMessageBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMQEventBus _publisher;
        public OutBoxMessageBackgroundService(IServiceProvider serviceProvider, IRabbitMQEventBus publisher)
        {
            _serviceProvider = serviceProvider;
            _publisher = publisher;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var _orderingContext = scope.ServiceProvider.GetService<TradingDbContext>();
            var messages = await _orderingContext.Set<OutBoxMessage>().Where(m => m.ProceddedOnUtc == null)
                .Take(10).ToListAsync(stoppingToken);
            foreach (var message in messages)
            {
                if (string.IsNullOrEmpty(message.Content))
                    continue;
                var retries = 3;
                var retry = Policy.Handle<Exception>()
                    .WaitAndRetry(
                    retries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retry, ctx) =>
                    {
                        Console.WriteLine($"发布时间失败:{message}");
                    });
                retry.Execute(() => _publisher.Publish(new { Content=message.Content,Id = message.Id }, exchange: "RabbitMQ.EventBus.Simple", routingKey: "rabbitmq.eventbus.test"));
                message.ProceddedOnUtc = DateTime.UtcNow;
            }
            await _orderingContext.SaveChangesAsync(stoppingToken);
        }
    }
}
