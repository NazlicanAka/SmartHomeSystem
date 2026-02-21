using SmartHome.API.Application.Interfaces;

namespace SmartHome.API.Infrastructure.Adapters
{
    public class WiFiAdapter : IDeviceProtocolAdapter
    {
        public string ProtocolName => "Wi-Fi";

        public async Task<bool> PairDeviceAsync(string deviceAddress)
        {
            // Gerçek bir senaryoda burada IP adresi üzerinden TCP/IP bağlantısı kurulur.
            // Biz burada 2 saniyelik bir gecikme ile ağ taraması ve bağlanma simülasyonu yapıyoruz.
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