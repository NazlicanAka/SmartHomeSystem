using SmartHome.API.Application.Interfaces;
using SmartHome.API.Domain.Enums;

namespace SmartHome.API.Infrastructure.Adapters
{
    public class BluetoothAdapter : IDeviceProtocolAdapter
    {
        public Protocol Protocol => Protocol.Bluetooth;

        public async Task<bool> PairDeviceAsync(string deviceAddress)
        {
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