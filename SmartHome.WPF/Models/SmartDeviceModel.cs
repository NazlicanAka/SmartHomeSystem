namespace SmartHome.WPF.Models
{
    // API'den dönen JSON verisini bu sınıfa dönüştüreceğiz (Deserialize)
    public class SmartDeviceModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; } // API'deki Enum 0(Işık), 1(Termostat) olarak gelir
        public bool IsOn { get; set; }
        public double? Temperature { get; set; } // Sadece termostatlarda dolu gelir

        // Ekranda daha güzel görünmesi için yardımcı bir özellik (Property)
        public string StatusText => IsOn ? "AÇIK" : "KAPALI";
    }
}