using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Interfaces;

namespace SmartHome.API.Domain.Devices
{
    public class SmartAirPurifier : ISmartDevice
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public DeviceType Type => DeviceType.AirPurifier;
        public bool IsOn { get; set; }

        public SmartAirPurifier(string name) { Name = name; }
        public void TurnOn() { IsOn = true; }
        public void TurnOff() { IsOn = false; }

        public string GetStatus()
        {
            return IsOn ? "Hava Temizleyici çalışıyor (Filtre: İyi)" : "Hava Temizleyici kapalı.";
        }
    }
}