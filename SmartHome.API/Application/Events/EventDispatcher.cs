using SmartHome.API.Domain.Events;

namespace SmartHome.API.Application.Events
{
    public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        // Olayı dinleyip aksiyon almak için gerekli metot
        Task HandleAsync(TEvent domainEvent);
    }

    public interface IEventDispatcher
    {
        // Haberi yayınlama metodu
        Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
    }

    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventDispatcher(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            // Her event için yeni bir scope oluştur (scoped service'leri kullanabilmek için)
            using var scope = _serviceScopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            // İlgili event için tüm handler'ları bul
            var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>();

            // Her handler'ı çalıştır (asenkron)
            var tasks = handlers.Select(handler => handler.HandleAsync(domainEvent));

            await Task.WhenAll(tasks);
        }
    }
}
