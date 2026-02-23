using SmartHome.API.Domain.Enums;

namespace SmartHome.API.Infrastructure.Data
{
    // Sadece veritabanındaki "Devices" tablosunu temsil eden sınıf
    public class DeviceEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DeviceType Type { get; set; }
        public bool IsOn { get; set; }
    }
}