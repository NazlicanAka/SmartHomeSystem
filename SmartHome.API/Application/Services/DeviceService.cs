using SmartHome.API.Application.Interfaces;
using SmartHome.API.Domain.Devices;
using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Interfaces;
using SmartHome.API.Infrastructure.Data;
using SmartHome.API.Application.Events;
using SmartHome.API.Domain.Events;
using SmartHome.API.Domain.Extensions;

namespace SmartHome.API.Application.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly SmartHomeDbContext _context;
        private readonly IEnumerable<IDeviceProtocolAdapter> _adapters;
        private readonly IEventDispatcher _eventDispatcher;

        public DeviceService(
            SmartHomeDbContext context, 
            IEnumerable<IDeviceProtocolAdapter> adapters,
            IEventDispatcher eventDispatcher)
        {
            _context = context;
            _adapters = adapters;
            _eventDispatcher = eventDispatcher;
        }

        // Her cihaz durum değişimini kaydeder
        private void LogDeviceAction(Guid deviceId, string deviceName, string action, string triggeredBy)
        {
            var log = new DeviceHistoryEntity
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                DeviceName = deviceName,
                Action = action,
                Timestamp = DateTime.UtcNow,
                TriggeredBy = triggeredBy
            };
            _context.DeviceHistory.Add(log);
        }


        public IEnumerable<ISmartDevice> GetAllDevices()
        {
            var entities = _context.Devices.ToList();
            var devices = new List<ISmartDevice>();

            // Entity'leri domain modeline dönüştür (mapping)
            foreach (var entity in entities)
            {
                if (entity.Type == DeviceType.Light)
                {
                    devices.Add(new SmartLight(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
                }
                else if (entity.Type == DeviceType.Thermostat)
                {
                    devices.Add(new SmartThermostat(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
                }
                else if (entity.Type == DeviceType.AirPurifier) 
                {
                    devices.Add(new SmartAirPurifier(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
                }
                else if (entity.Type == DeviceType.RobotVacuum)
                {
                    devices.Add(new SmartRobotVacuum(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
                }
            }
            return devices;
        }

        public async Task AddCustomDeviceAsync(string name, DeviceType type, string protocol, string username)
        {
            // 1. İstenen protokole uygun adaptörü bul
            var adapter = _adapters.FirstOrDefault(a => a.Protocol.ToDisplayString() == protocol);

            if (adapter != null)
            {
                // 2. Eşleşme Simülasyonunu Başlat (Sahte bir MAC adresi gönderiyoruz)
                bool isPaired = await adapter.PairDeviceAsync("AA:BB:CC:DD:EE");

                if (isPaired)
                {
                    // 3. Eşleşme başarılıysa veritabanına kaydet
                    var entity = new DeviceEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        Type = type,
                        IsOn = false
                    };
                    _context.Devices.Add(entity);

                    LogDeviceAction(entity.Id, name, "Eklendi", username);

                    _context.SaveChanges();

                    // Cihaz eklendi event'i yayınla
                    await _eventDispatcher.PublishAsync(new DeviceAddedEvent(
                        entity.Id, name, type, protocol, username));
                }
            }
        }

        public async Task RemoveDeviceAsync(Guid id, string username)
        {
            var entity = _context.Devices.FirstOrDefault(d => d.Id == id);
            if (entity != null)
            {
                var deviceName = entity.Name;

                LogDeviceAction(entity.Id, entity.Name, "Silindi", username);

                _context.Devices.Remove(entity);
                _context.SaveChanges();

                // Cihaz silindi event'i yayınla
                await _eventDispatcher.PublishAsync(new DeviceRemovedEvent(
                    id, deviceName, username));
            }
        }

        public void TurnOnAllDevices()
        {
            var entities = _context.Devices.ToList();
            foreach (var entity in entities)
            {
                entity.IsOn = true;
            }
            _context.SaveChanges();
        }

        public void TurnOffAllDevices()
        {
            var entities = _context.Devices.ToList();
            foreach (var entity in entities)
            {
                entity.IsOn = false;
            }
            _context.SaveChanges();
        }

        public async Task ToggleDeviceAsync(Guid id, string username)
        {
            var entity = _context.Devices.FirstOrDefault(d => d.Id == id);
            if (entity != null)
            {
                bool previousState = entity.IsOn;
                entity.IsOn = !entity.IsOn;

                string action = entity.IsOn ? "Açıldı" : "Kapatıldı";
                LogDeviceAction(entity.Id, entity.Name, action, username);

                _context.SaveChanges();

                // Cihaz durumu değişti (Otomasyon handler'ı bu event'i dinleyecek)
                await _eventDispatcher.PublishAsync(new DeviceStateChangedEvent(
                    entity.Id, entity.Name, entity.Type, entity.IsOn, previousState, username, "User"));
            }
        }

        public async Task TriggerPresenceAsync(bool isHome, string username)
        {
            var devices = _context.Devices.ToList();
            string presenceTrigger = isHome ? $"Presence ({username}): Eve Gelindi" : $"Presence ({username}): Evden Ayrılındı";
            int affectedCount = 0;

            foreach (var device in devices)
            {
                // Işıklar ve Termostat açılsın
                if (isHome)
                {
                    if (device.Type == SmartHome.API.Domain.Enums.DeviceType.Light || device.Type == SmartHome.API.Domain.Enums.DeviceType.Thermostat)
                    {
                        device.IsOn = true;
                        LogDeviceAction(device.Id, device.Name, "Açıldı", presenceTrigger);
                        affectedCount++;
                    }
                }
                // Işıklar ve Termostat kapansın
                else
                {
                    if (device.Type == SmartHome.API.Domain.Enums.DeviceType.Light || device.Type == SmartHome.API.Domain.Enums.DeviceType.Thermostat)
                    {
                        device.IsOn = false;
                        LogDeviceAction(device.Id, device.Name, "Kapatıldı", presenceTrigger);
                        affectedCount++;
                    }
                }
            }
            _context.SaveChanges();

            // Kullanıcı presence değişti
            await _eventDispatcher.PublishAsync(new UserPresenceChangedEvent(
                username, isHome, affectedCount));

            await Task.CompletedTask; // async metod olduğu için
        }

        public IEnumerable<DeviceHistoryEntity> GetDeviceHistory(Guid? deviceId = null)
        {
            if (deviceId.HasValue)
            {
                // Belirli bir cihazın geçmişi
                return _context.DeviceHistory
                    .Where(h => h.DeviceId == deviceId.Value)
                    .OrderByDescending(h => h.Timestamp)
                    .ToList();
            }
            else
            {
                // Tüm cihazların geçmişi
                return _context.DeviceHistory
                    .OrderByDescending(h => h.Timestamp)
                    .ToList();
            }
        }

        public void ClearAllHistory()
        {
            var allHistory = _context.DeviceHistory.ToList();
            _context.DeviceHistory.RemoveRange(allHistory);
            _context.SaveChanges();
        }

        // Belirli türdeki tüm cihazları aç/kapat (Automation handler için)
        public async Task<List<Guid>> ToggleDevicesByTypeAsync(DeviceType deviceType, bool turnOn, string triggeredBy)
        {
            var devices = _context.Devices.Where(d => d.Type == deviceType).ToList();
            var affectedIds = new List<Guid>();

            foreach (var device in devices)
            {
                if (device.IsOn != turnOn) // Sadece değişecekse işlem yap
                {
                    device.IsOn = turnOn;
                    string action = turnOn ? "Açıldı" : "Kapatıldı";
                    LogDeviceAction(device.Id, device.Name, action, triggeredBy);
                    affectedIds.Add(device.Id);

                    // Her cihaz için ayrı event yayınla (SignalR için)
                    await _eventDispatcher.PublishAsync(new DeviceStateChangedEvent(
                        device.Id, device.Name, device.Type, device.IsOn, !device.IsOn, triggeredBy, "Automation"));
                }
            }

            _context.SaveChanges();
            return affectedIds;
        }

        // Enerji Tasarrufu: Açık unutulmuş ışıkları kapat
        public async Task TriggerEnergySavingAsync()
        {
            // database'den açık ışıkları çektim.
            var lights = _context.Devices
                .Where(d => d.Type == DeviceType.Light && d.IsOn)
                .ToList();

            var affectedIds = new List<Guid>();

            foreach (var light in lights)
            {
                // ışıkları kapattım, log attım
                light.IsOn = false;
                LogDeviceAction(light.Id, light.Name, "Kapatıldı", "Sistem (Enerji Tasarrufu)");
                affectedIds.Add(light.Id);
            }
            _context.SaveChanges();

            foreach (var light in lights)
            {
                await _eventDispatcher.PublishAsync(new DeviceStateChangedEvent(
                    light.Id, light.Name, light.Type, false, true, "Sistem", "EnergySaving"));
            }

            // Toplu enerji tasarrufu event'i yayınla
            if (affectedIds.Any())
            {
                await _eventDispatcher.PublishAsync(new EnergySavingTriggeredEvent(
                    affectedIds.Count, affectedIds));
            }
        }
    }
}