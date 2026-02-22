using Microsoft.AspNetCore.SignalR;

namespace SmartHome.API.Hubs
{
    /// <summary>
    /// SignalR Hub - Gerçek zamanlı bildirimler için
    /// Client'lar bu hub'a bağlanır ve server-side event'leri dinler
    /// </summary>
    public class DeviceNotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name ?? "Guest";
            Console.WriteLine($"🔌 SignalR: {username} bağlandı (ConnectionId: {Context.ConnectionId})");
            
            await Clients.Caller.SendAsync("Connected", "SignalR bağlantısı kuruldu!");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User?.Identity?.Name ?? "Guest";
            Console.WriteLine($"❌ SignalR: {username} bağlantıyı kesti");
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Client'lar bu metodu çağırarak belirli bir gruba katılabilir
        /// </summary>
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            Console.WriteLine($"👥 {Context.User?.Identity?.Name} joined room: {roomName}");
        }
    }
}
