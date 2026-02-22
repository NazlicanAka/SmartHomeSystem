namespace SmartHome.API.Application.Interfaces
{
    /// <summary>
    /// Gelişmiş Cihaz Adapter Interface
    /// </summary>
    public interface IAdvancedDeviceProtocolAdapter : IDeviceProtocolAdapter
    {
        // Cihaz keşfi (Discovery) - Ağdaki cihazları tarar
        Task<IEnumerable<DiscoveredDevice>> DiscoverDevicesAsync(TimeSpan timeout);

        // Cihaz durumunu kontrol eder (Health Check)
        Task<DeviceHealthStatus> CheckDeviceHealthAsync(string deviceAddress);

        // Retry mekanizması ile komut gönderme
        Task<bool> SendCommandWithRetryAsync(string deviceAddress, string command, int maxRetries = 3);

        // Cihazdan durum okuma (status query)
        Task<DeviceStatus> GetDeviceStatusAsync(string deviceAddress);

        // Bağlantı süresi (connection timeout)
        TimeSpan ConnectionTimeout { get; }

        // Desteklenen komutlar listesi
        IEnumerable<string> SupportedCommands { get; }
    }

    // Keşfedilen cihaz bilgisi
    public class DiscoveredDevice
    {
        public string Address { get; set; } // MAC veya IP
        public string Name { get; set; }
        public string DeviceType { get; set; }
        public int SignalStrength { get; set; } // dBm cinsinden
        public string FirmwareVersion { get; set; }
    }

    // Cihaz sağlık durumu
    public class DeviceHealthStatus
    {
        public bool IsOnline { get; set; }
        public int ResponseTime { get; set; } // milliseconds
        public int SignalQuality { get; set; } // 0-100
        public DateTime LastSeen { get; set; }
        public string ErrorMessage { get; set; }
    }

    // Cihaz durumu
    public class DeviceStatus
    {
        public bool IsConnected { get; set; }
        public bool IsOn { get; set; }
        public int BatteryLevel { get; set; } // 0-100 (varsa)
        public Dictionary<string, object> CustomProperties { get; set; }
    }
}
