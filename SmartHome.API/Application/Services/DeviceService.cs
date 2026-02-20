using SmartHome.API.Application.Interfaces;
using SmartHome.API.Domain.Interfaces;

namespace SmartHome.API.Application.Services
{
    public class DeviceService : IDeviceService
    {
        // İleride veritabanı (EF Core) buraya gelecek. Şimdilik RAM'de (List) tutuyoruz.
        private readonly List<ISmartDevice> _devices;

        public DeviceService()
        {
            _devices = new List<ISmartDevice>();
        }

        public IEnumerable<ISmartDevice> GetAllDevices()
        {
            return _devices;
        }

        public void AddDevice(ISmartDevice device)
        {
            _devices.Add(device);
        }

        public void TurnOnAllDevices()
        {
            // İşte ISmartDevice şablonunun gücü! Cihaz ışık mı termostat mı diye 
            // if-else yazmadan hepsini tek bir döngüde açabiliyoruz.
            foreach (var device in _devices)
            {
                device.TurnOn();
            }
        }

        public void TurnOffAllDevices()
        {
            foreach (var device in _devices)
            {
                device.TurnOff();
            }
        }

        public void ToggleDevice(Guid id)
        {
            // Cihazı listeden ID'sine göre bul
            var device = _devices.FirstOrDefault(d => d.Id == id);
            if (device != null)
            {
                // Açıksa kapat, kapalıysa aç
                if (device.IsOn)
                    device.TurnOff();
                else
                    device.TurnOn();
            }
        }
    }
}