namespace SmartHome.API.Infrastructure.Data
{
    // Cihaz geçmişi: Her durum değişimini kaydeder
    public class DeviceHistoryEntity
    {
        public Guid Id { get; set; } // Log kaydının benzersiz ID'si
        public Guid DeviceId { get; set; } // Hangi cihaz
        public string DeviceName { get; set; } // Cihaz adı (kolay okumak için)
        public string Action { get; set; } // "Açıldı", "Kapatıldı", "Eklendi", "Silindi"
        public DateTime Timestamp { get; set; } // Ne zaman oldu
        public string TriggeredBy { get; set; } // "Kullanıcı", "Otomasyon", "Presence"
    }
}
