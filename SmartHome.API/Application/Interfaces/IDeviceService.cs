using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Interfaces;
using SmartHome.API.Infrastructure.Data;

namespace SmartHome.API.Application.Interfaces
{
    public interface IDeviceService
    {
        IEnumerable<ISmartDevice> GetAllDevices();

        void TurnOnAllDevices();

        void TurnOffAllDevices();

        Task ToggleDeviceAsync(Guid id, string username);

        Task AddCustomDeviceAsync(string name, DeviceType type, string protocol, string username);
        Task RemoveDeviceAsync(Guid id, string username);

        Task TriggerPresenceAsync(bool isHome, string username);

        IEnumerable<DeviceHistoryEntity> GetDeviceHistory(Guid? deviceId = null);

        void ClearAllHistory();

        Task<List<Guid>> ToggleDevicesByTypeAsync(DeviceType deviceType, bool turnOn, string triggeredBy);

        Task TriggerEnergySavingAsync();
    }
}