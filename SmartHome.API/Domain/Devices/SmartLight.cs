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
            // Case study notu: Gerçekte burada Wi-Fi modülüne (IoT) "Açıl" sinyali gönderilir. 
            // Biz mimariyi gösterdiğimiz için Mock (sahte) olarak sadece durumu güncelliyoruz.
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