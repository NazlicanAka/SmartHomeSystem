using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Interfaces;
using SmartHome.API.Infrastructure.Data;

namespace SmartHome.API.Application.Interfaces
{
    public interface IDeviceService
    {
        // Sistemdeki tüm cihazları listeler
        IEnumerable<ISmartDevice> GetAllDevices();

        // Sisteme yeni bir cihaz ekler
        void AddDevice(ISmartDevice device);

        // Senaryo 1: Tüm cihazları aç
        void TurnOnAllDevices();

        // Senaryo 2: Tüm cihazları kapat
        void TurnOffAllDevices();

        // Belirli bir cihazı aç/kapat (Toggle)
        Task ToggleDeviceAsync(Guid id, string username);

        Task AddCustomDeviceAsync(string name, DeviceType type, string protocol, string username);
        Task RemoveDeviceAsync(Guid id, string username);

        Task TriggerPresenceAsync(bool isHome, string username);

        // 📊 Cihaz geçmişini getir
        IEnumerable<DeviceHistoryEntity> GetDeviceHistory(Guid? deviceId = null);

        // 🗑️ Tüm geçmişi temizle
        void ClearAllHistory();
    }
}