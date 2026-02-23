using Microsoft.AspNetCore.SignalR;
using SmartHome.API.Application.Events;
using SmartHome.API.Domain.Events;
using SmartHome.API.Hubs;

namespace SmartHome.API.Application.EventHandlers
{

    // Ekrana anında yansımasını sağlayan yer.
    // Uygulamaya bağlı herkes aynı anda değişiklikleri görecek.(Broadcast)
    public class SignalRNotificationHandler :
        IEventHandler<DeviceStateChangedEvent>,
        IEventHandler<DeviceAddedEvent>,
        IEventHandler<DeviceRemovedEvent>,
        IEventHandler<AutomationTriggeredEvent>,
        IEventHandler<UserPresenceChangedEvent>,
        IEventHandler<EnergySavingTriggeredEvent>
    {
        private readonly IHubContext<DeviceNotificationHub> _hubContext;

        public SignalRNotificationHandler(IHubContext<DeviceNotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task HandleAsync(DeviceStateChangedEvent domainEvent)
        {
            // domainEvent içindeki bilgileri kullanarak anlaşılır, kullanılabilir bir mesaj oluşturuyoruz.
            var message = new
            {
                Type = "DeviceStateChanged",
                DeviceId = domainEvent.DeviceId,
                DeviceName = domainEvent.DeviceName,
                IsOn = domainEvent.IsOn,
                ChangedBy = domainEvent.ChangedBy,
                Timestamp = domainEvent.OccurredAt,
                Message = $"{domainEvent.DeviceName} {(domainEvent.IsOn ? "açıldı" : "kapandı")}"
            };

            await _hubContext.Clients.All.SendAsync("DeviceStateChanged", message); // Diğer clientlara bu mesajı gönderiyoruz.
        }

        public async Task HandleAsync(DeviceAddedEvent domainEvent)
        {
            var message = new
            {
                Type = "DeviceAdded",
                DeviceId = domainEvent.DeviceId,
                DeviceName = domainEvent.DeviceName,
                DeviceType = domainEvent.DeviceType.ToString(),
                AddedBy = domainEvent.AddedBy,
                Timestamp = domainEvent.OccurredAt,
                Message = $"Yeni cihaz eklendi: {domainEvent.DeviceName}"
            };

            await _hubContext.Clients.All.SendAsync("DeviceAdded", message);
        }

        public async Task HandleAsync(DeviceRemovedEvent domainEvent)
        {
            var message = new
            {
                Type = "DeviceRemoved",
                DeviceId = domainEvent.DeviceId,
                DeviceName = domainEvent.DeviceName,
                RemovedBy = domainEvent.RemovedBy,
                Timestamp = domainEvent.OccurredAt,
                Message = $"Cihaz silindi: {domainEvent.DeviceName}"
            };

            await _hubContext.Clients.All.SendAsync("DeviceRemoved", message);
        }

        public async Task HandleAsync(AutomationTriggeredEvent domainEvent)
        {
            var message = new
            {
                Type = "AutomationTriggered",
                AutomationName = domainEvent.AutomationName,
                TriggerSource = domainEvent.TriggerSource,
                AffectedDeviceCount = domainEvent.AffectedDevices.Count,
                Timestamp = domainEvent.OccurredAt,
                Message = $"Otomasyon çalıştı: {domainEvent.AutomationName}"
            };

            await _hubContext.Clients.All.SendAsync("AutomationTriggered", message);
        }

        public async Task HandleAsync(UserPresenceChangedEvent domainEvent)
        {
            var message = new
            {
                Type = "UserPresenceChanged",
                Username = domainEvent.Username,
                IsHome = domainEvent.IsHome,
                AffectedDeviceCount = domainEvent.AffectedDeviceCount,
                Timestamp = domainEvent.OccurredAt,
                Message = $"{domainEvent.Username} {(domainEvent.IsHome ? "eve geldi" : "evden ayrıldı")}"
            };

            await _hubContext.Clients.All.SendAsync("UserPresenceChanged", message);
        }

        public async Task HandleAsync(EnergySavingTriggeredEvent domainEvent)
        {
            var message = new
            {
                Type = "EnergySavingTriggered",
                DevicesAffected = domainEvent.DevicesAffected,
                Timestamp = domainEvent.OccurredAt,
                Message = $"Enerji Tasarrufu: {domainEvent.DevicesAffected} ışık kapatıldı"
            };

            await _hubContext.Clients.All.SendAsync("EnergySavingTriggered", message);
        }
    }
}
