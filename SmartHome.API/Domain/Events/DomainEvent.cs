namespace SmartHome.API.Domain.Events
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTime OccurredAt { get; }
        string EventType { get; }
    }

    // Sadece bir event oldu denilemez, ışık kapandı, cihaz eklendi, hareket algılandı gibi farklı eventler olabilir. O yüzden abstract yapıyoruz.
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid EventId { get; }
        public DateTime OccurredAt { get; }
        public string EventType { get; }

        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            EventType = GetType().Name; // O an kullanan sınıfın adını okur ve onun adını EventType olarak koyar.
        }
    }
}
