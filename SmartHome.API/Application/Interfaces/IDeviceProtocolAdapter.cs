namespace SmartHome.API.Application.Interfaces
{
    // Cihazlarla konuşacak tüm adaptörlerin ortak sözleşmesi
    public interface IDeviceProtocolAdapter
    {
        // Adaptörün hangi protokolle çalıştığını belirtir (Örn: "Wi-Fi" veya "Bluetooth")
        string ProtocolName { get; }

        // Cihazla ilk eşleşme (Pairing) işlemini yapar
        Task<bool> PairDeviceAsync(string deviceAddress);

        // Cihaza aç/kapat gibi komutlar gönderir
        Task<bool> SendCommandAsync(string deviceAddress, string command);
    }
}