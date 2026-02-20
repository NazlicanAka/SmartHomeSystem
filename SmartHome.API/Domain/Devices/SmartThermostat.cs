using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Interfaces;

namespace SmartHome.API.Domain.Devices
{
    public class SmartThermostat : ISmartDevice
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public DeviceType Type => DeviceType.Thermostat;
        public bool IsOn { get; private set; }

        // Termostata özel ekstra özellik:
        public double Temperature { get; private set; } = 22.0;

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
            return IsOn ? $"Termostat çalışıyor. Sıcaklık: {Temperature}°C" : "Termostat kapalı.";
        }

        // ISmartDevice'da olmayan, sadece bu cihaza has bir metot
        public void SetTemperature(double targetTemp)
        {
            Temperature = targetTemp;
        }
    }
}