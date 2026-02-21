using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Interfaces;

namespace SmartHome.API.Domain.Devices
{
    public class SmartRobotVacuum : ISmartDevice
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public DeviceType Type => DeviceType.RobotVacuum;
        public bool IsOn { get; set; }

        public SmartRobotVacuum(string name) { Name = name; }
        public void TurnOn() { IsOn = true; }
        public void TurnOff() { IsOn = false; }

        public string GetStatus()
        {
            return IsOn ? "Robot Süpürge temizlik yapıyor (Şarj: %80)" : "Robot Süpürge şarj istasyonunda.";
        }
    }
}