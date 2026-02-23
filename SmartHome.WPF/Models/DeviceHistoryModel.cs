namespace SmartHome.WPF.Models
{
    public class DeviceHistoryModel
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string TriggeredBy { get; set; }
        public string FormattedTime => Timestamp.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
    }
}
