using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Interfaces;

namespace SmartHome.API.Domain.Devices
{
    public class SmartLight : ISmartDevice
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Otomatik benzersiz ID üretir
        public string Name { get; set; }
        public DeviceType Type => DeviceType.Light;
        public bool IsOn { get; set; }



        public SmartLight(string name)
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
            return IsOn ? "Işık şu an AÇIK." : "Işık şu an KAPALI.";
        }
    }
}