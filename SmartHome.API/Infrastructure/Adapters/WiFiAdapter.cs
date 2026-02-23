using SmartHome.API.Application.Interfaces;
using SmartHome.API.Domain.Enums;

namespace SmartHome.API.Infrastructure.Adapters
{
    public class WiFiAdapter : IDeviceProtocolAdapter
    {
        public Protocol Protocol => Protocol.WiFi;

        public async Task<bool> PairDeviceAsync(string deviceAddress)
        {
            // 2 saniyelik bir gecikme ile ağ taraması ve bağlanma simülasyonu yapıyoruz.
            await Task.Delay(2000);
            return true; // Başarıyla eşleşti
        }

        public async Task<bool> SendCommandAsync(string deviceAddress, string command)
        {
            // Komut gönderme simülasyonu
            await Task.Delay(500);
            return true;
        }
    }
}