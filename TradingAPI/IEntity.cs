using RabbitMQ.EventBus.AspNetCore.Events;

namespace TradingAPI
{
    public abstract class IEntity
    {
        private int id;
        public virtual int Id
        {
            get { return id; }
            protected set { id = value; }
        }

        private List<IEvent> _domainEvents;
        public IReadOnlyCollection<IEvent> DomainEvents => _domainEvents?.AsReadOnly();
        public void AddDomainEvent(IEvent eventItem)
        {
            _domainEvents = _domainEvents ?? new List<IEvent>();
            _domainEvents.Add(eventItem);
        }
        public void RemoveDomainEvent(IEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }
        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}
