namespace SmartHome.API.Domain.Events
{
    /// <summary>
    /// Tüm domain event'lerinin türeyeceği temel interface
    /// </summary>
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTime OccurredAt { get; }
        string EventType { get; }
    }

    /// <summary>
    /// Domain Event base class
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid EventId { get; }
        public DateTime OccurredAt { get; }
        public string EventType { get; }

        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            EventType = GetType().Name;
        }
    }
}
