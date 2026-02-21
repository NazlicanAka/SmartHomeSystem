using SmartHome.API.Application.Interfaces;

namespace SmartHome.API.Infrastructure.Adapters
{
    public class BluetoothAdapter : IDeviceProtocolAdapter
    {
        public string ProtocolName => "Bluetooth";

        public async Task<bool> PairDeviceAsync(string deviceAddress)
        {
            // Bluetooth eşleşme simülasyonu (Wi-Fi'a göre biraz daha hızlı bağlanıyor gibi yapalım)
            await Task.Delay(1000);
            return true;
        }

        public async Task<bool> SendCommandAsync(string deviceAddress, string command)
        {
            await Task.Delay(500);
            return true;
        }
    }
}