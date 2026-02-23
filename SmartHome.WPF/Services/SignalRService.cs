using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SmartHome.WPF.Services
{
    // SignalR bağlantı yönetimi ve gerçek zamanlı bildirimler
    public class SignalRService
    {
        private HubConnection? _connection;
        private readonly string _hubUrl = "https://localhost:7106/hubs/notifications";

        // Event'ler - ViewModel'ler bu event'leri dinleyecek
        public event EventHandler<DeviceNotificationEventArgs>? DeviceStateChanged;
        public event EventHandler<DeviceNotificationEventArgs>? DeviceAdded;
        public event EventHandler<DeviceNotificationEventArgs>? DeviceRemoved;
        public event EventHandler<string>? AutomationTriggered;
        public event EventHandler<string>? UserPresenceChanged;
        public event EventHandler<string>? EnergySavingTriggered;
        public event EventHandler<ConnectionState>? ConnectionStateChanged;

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        public SignalRService()
        {
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect() // Otomatik yeniden bağlanma
                .Build();

            // Bağlantı durumu değişikliklerini izle
            _connection.Closed += async (error) =>
            {
                ConnectionStateChanged?.Invoke(this, ConnectionState.Disconnected);
                await Task.Delay(5000); // 5 saniye bekle
                await StartAsync(); // Tekrar bağlan
            };

            _connection.Reconnecting += (error) =>
            {
                ConnectionStateChanged?.Invoke(this, ConnectionState.Reconnecting);
                return Task.CompletedTask;
            };

            _connection.Reconnected += (connectionId) =>
            {
                ConnectionStateChanged?.Invoke(this, ConnectionState.Connected);
                return Task.CompletedTask;
            };

            // SignalR event handler'larını kaydet
            RegisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            if (_connection == null) return;

            // DeviceStateChanged event'i dinle
            _connection.On<object>("DeviceStateChanged", (message) =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(message);
                var notification = System.Text.Json.JsonSerializer.Deserialize<DeviceNotificationEventArgs>(json);
                
                DeviceStateChanged?.Invoke(this, notification ?? new DeviceNotificationEventArgs());
            });

            // DeviceAdded event'i dinle
            _connection.On<object>("DeviceAdded", (message) =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(message);
                var notification = System.Text.Json.JsonSerializer.Deserialize<DeviceNotificationEventArgs>(json);
                
                DeviceAdded?.Invoke(this, notification ?? new DeviceNotificationEventArgs());
            });

            // DeviceRemoved event'i dinle
            _connection.On<object>("DeviceRemoved", (message) =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(message);
                var notification = System.Text.Json.JsonSerializer.Deserialize<DeviceNotificationEventArgs>(json);
                
                DeviceRemoved?.Invoke(this, notification ?? new DeviceNotificationEventArgs());
            });

            // AutomationTriggered event'i dinle
            _connection.On<object>("AutomationTriggered", (message) =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(message);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var msg = doc.RootElement.GetProperty("Message").GetString() ?? "Otomasyon tetiklendi";
                
                AutomationTriggered?.Invoke(this, msg);
            });

            // UserPresenceChanged event'i dinle
            _connection.On<object>("UserPresenceChanged", (message) =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(message);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var msg = doc.RootElement.GetProperty("Message").GetString() ?? "Kullanıcı presence değişti";

                UserPresenceChanged?.Invoke(this, msg);
            });

            // EnergySavingTriggered event'i dinle
            _connection.On<object>("EnergySavingTriggered", (message) =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(message);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var msg = doc.RootElement.GetProperty("Message").GetString() ?? "Enerji tasarrufu aktif";

                EnergySavingTriggered?.Invoke(this, msg);
            });
        }

        public async Task StartAsync()
        {
            if (_connection == null) return;

            try
            {
                if (_connection.State == HubConnectionState.Disconnected)
                {
                    await _connection.StartAsync();
                    ConnectionStateChanged?.Invoke(this, ConnectionState.Connected);
                    Console.WriteLine("SignalR bağlantısı kuruldu!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR bağlantı hatası: {ex.Message}");
                ConnectionStateChanged?.Invoke(this, ConnectionState.Failed);
            }
        }

        public async Task StopAsync()
        {
            if (_connection != null && _connection.State == HubConnectionState.Connected)
            {
                await _connection.StopAsync();
                ConnectionStateChanged?.Invoke(this, ConnectionState.Disconnected);
            }
        }

        public async Task SendMessageAsync(string method, object data)
        {
            if (_connection != null && IsConnected)
            {
                await _connection.InvokeAsync(method, data);
            }
        }
    }

    // SignalR'dan gelen bildirim verisi
    public class DeviceNotificationEventArgs : EventArgs
    {
        public string Type { get; set; } = "";
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; } = "";
        public bool IsOn { get; set; }
        public string ChangedBy { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }

    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Reconnecting,
        Failed
    }
}
