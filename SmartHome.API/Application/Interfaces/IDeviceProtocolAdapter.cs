using SmartHome.API.Domain.Enums;

namespace SmartHome.API.Application.Interfaces
{
    public interface IDeviceProtocolAdapter
    {
        Protocol Protocol { get; }

        Task<bool> PairDeviceAsync(string deviceAddress);

        Task<bool> SendCommandAsync(string deviceAddress, string command);
    }
}