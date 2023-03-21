using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using RabbitMQ.EventBus.AspNetCore.Events;
using System.Diagnostics.CodeAnalysis;

namespace TradingAPI
{
    public sealed class OutBoxMessageInterceptor:SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            DbContext? dbContxt = eventData.Context;
            if (dbContxt is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }
            var events = dbContxt.ChangeTracker.Entries<IEntity>().Select(x => x.Entity).SelectMany(x =>
            {
                List<IEvent> entities = new List<IEvent>();
                foreach (var item in x.DomainEvents)
                {
                    if(!(item is null))
                    entities.Add(item);
                }
                x.ClearDomainEvents();
                return entities;
            }).Select(x => new OutBoxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = x.GetType().Name,
                Content = System.Text.Json.JsonSerializer.Serialize((CreateOrderEvent)x)
            }).ToList();
            if(events!=null && events.Any())
             dbContxt.Set<OutBoxMessage>().AddRangeAsync(events);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
