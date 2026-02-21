using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Interfaces;

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
        void ToggleDevice(Guid id);

        void AddCustomDevice(string name, DeviceType type);
        void RemoveDevice(Guid id);
    }
}