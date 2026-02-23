using SmartHome.API.Domain.Enums;

namespace SmartHome.API.Domain.Extensions
{
    public static class ProtocolExtensions
    {
        public static string ToDisplayString(this Protocol protocol)
        {
            return protocol switch
            {
                Protocol.WiFi => "WiFi",
                Protocol.Bluetooth => "Bluetooth",
                _ => protocol.ToString()
            };
        }
    }
}
