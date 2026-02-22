using SmartHome.API.Application.Events;
using SmartHome.API.Domain.Events;

namespace SmartHome.API.Application.EventHandlers
{
    /// <summary>
    /// Cihaz durumu değiştiğinde loglama yapar
    /// </summary>
    public class DeviceStateChangedLoggingHandler : IEventHandler<DeviceStateChangedEvent>
    {
        public Task HandleAsync(DeviceStateChangedEvent domainEvent)
        {
            var action = domainEvent.IsOn ? "AÇILDI" : "KAPATILDI";
            Console.WriteLine($"📝 [LOG] {domainEvent.DeviceName} {action} (Kullanan: {domainEvent.ChangedBy}, Sebep: {domainEvent.Reason})");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Yeni cihaz eklendiğinde bildirim gönderir
    /// </summary>
    public class DeviceAddedNotificationHandler : IEventHandler<DeviceAddedEvent>
    {
        public Task HandleAsync(DeviceAddedEvent domainEvent)
        {
            Console.WriteLine($"🔔 [NOTIFICATION] Yeni cihaz eklendi: {domainEvent.DeviceName} ({domainEvent.DeviceType}) - {domainEvent.AddedBy} tarafından");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Cihaz silindiğinde bildirim gönderir
    /// </summary>
    public class DeviceRemovedNotificationHandler : IEventHandler<DeviceRemovedEvent>
    {
        public Task HandleAsync(DeviceRemovedEvent domainEvent)
        {
            Console.WriteLine($"🗑️ [NOTIFICATION] Cihaz silindi: {domainEvent.DeviceName} - {domainEvent.RemovedBy} tarafından");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Otomasyon tetiklendiğinde bildirim gönderir
    /// </summary>
    public class AutomationTriggeredNotificationHandler : IEventHandler<AutomationTriggeredEvent>
    {
        public Task HandleAsync(AutomationTriggeredEvent domainEvent)
        {
            Console.WriteLine($"🤖 [AUTOMATION] {domainEvent.AutomationName} çalıştı - {domainEvent.AffectedDevices.Count} cihaz etkilendi");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Kullanıcı presence değiştiğinde bildirim gönderir
    /// </summary>
    public class UserPresenceChangedNotificationHandler : IEventHandler<UserPresenceChangedEvent>
    {
        public Task HandleAsync(UserPresenceChangedEvent domainEvent)
        {
            var status = domainEvent.IsHome ? "eve geldi" : "evden ayrıldı";
            Console.WriteLine($"🏠 [PRESENCE] {domainEvent.Username} {status} - {domainEvent.AffectedDeviceCount} cihaz güncellendi");
            return Task.CompletedTask;
        }
    }
}
