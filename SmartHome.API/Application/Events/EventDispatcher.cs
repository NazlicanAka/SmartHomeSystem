namespace SmartHome.API.Application.Events
{
    /// <summary>
    /// Event Handler interface - Her event handler bu interface'i implement eder
    /// </summary>
    public interface IEventHandler<in TEvent> where TEvent : SmartHome.API.Domain.Events.IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent);
    }

    /// <summary>
    /// Event Dispatcher (Message Bus) - Event'leri ilgili handler'lara yönlendirir
    /// </summary>
    public interface IEventDispatcher
    {
        Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : SmartHome.API.Domain.Events.IDomainEvent;
    }

    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : SmartHome.API.Domain.Events.IDomainEvent
        {
            // İlgili event için tüm handler'ları bul
            var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();

            // Her handler'ı çalıştır (asenkron)
            var tasks = handlers.Select(handler => handler.HandleAsync(domainEvent));
            
            await Task.WhenAll(tasks);

            // Console'a log yazdır (production'da proper logging kullanılır)
            Console.WriteLine($"📢 Event Published: {domainEvent.EventType} at {domainEvent.OccurredAt}");
        }
    }
}
