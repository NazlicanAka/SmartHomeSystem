using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SmartHome.API.Hubs
{
    // SignalR Hub - Gerçek zamanlı bildirimler için
    // Client'lar bu hub'a bağlanır ve server-side event'leri dinler
    public class DeviceNotificationHub : Hub
    {
        private readonly ILogger<DeviceNotificationHub> _logger;

        public DeviceNotificationHub(ILogger<DeviceNotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name ?? "Guest";
            _logger.LogInformation("SignalR: {Username} bağlandı (ConnectionId: {ConnectionId})", 
                username, Context.ConnectionId);

            await Clients.Caller.SendAsync("Connected", new 
            { 
                Message = "SignalR bağlantısı kuruldu!",
                ConnectionId = Context.ConnectionId,
                ServerTime = DateTime.UtcNow
            });

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User?.Identity?.Name ?? "Guest";

            if (exception != null)
            {
                _logger.LogWarning("SignalR: {Username} bağlantıyı kesti (Hata: {Error})", 
                    username, exception.Message);
            }
            else
            {
                _logger.LogInformation("❌ SignalR: {Username} bağlantıyı kesti", username);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
