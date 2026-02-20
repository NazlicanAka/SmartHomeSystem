using SmartHome.API.Domain.Enums;

namespace SmartHome.API.Domain.Interfaces
{
    public interface ISmartDevice
    {
        Guid Id { get; } // Cihazın benzersiz numarası
        string Name { get; set; } // Örn: "Oturma Odası Işığı"
        DeviceType Type { get; } // Cihazın türü (Enum'dan gelecek)
        bool IsOn { get; } // Cihaz açık mı kapalı mı?

        // Her cihazın yapmak zorunda olduğu ortak eylemler:
        void TurnOn();
        void TurnOff();
        string GetStatus(); // Cihazın o anki durumunu (örn: "Sıcaklık 22 derece") döndürür
    }
}