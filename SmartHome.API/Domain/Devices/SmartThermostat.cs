using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Interfaces;

namespace SmartHome.API.Domain.Devices
{
    public class SmartThermostat : ISmartDevice
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public DeviceType Type => DeviceType.Thermostat;
        public bool IsOn { get; set; }

        public SmartThermostat(string name)
        {
            Name = name;
        }

        public void TurnOn()
        {
            IsOn = true;
        }

        public void TurnOff()
        {
            IsOn = false;
        }

        public string GetStatus()
        {
            return IsOn ? "Termostat çalışıyor." : "Termostat kapalı.";
        }
    }
}