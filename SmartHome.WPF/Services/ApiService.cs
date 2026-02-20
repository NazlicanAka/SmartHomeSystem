using System.Net.Http;
using System.Net.Http.Json; // JSON verilerini otomatik okumak için
using SmartHome.WPF.Models;

namespace SmartHome.WPF.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        // Buradaki port numarasını (7106), kendi siyah ekranında yazan numarayla değiştir!
        private const string BaseUrl = "https://localhost:7106/api/devices";

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        // 1. Postman'deki GET İsteği: Cihazları Listele
        public async Task<List<SmartDeviceModel>> GetDevicesAsync()
        {
            try
            {
                // API'den gelen JSON'u doğrudan SmartDeviceModel listesine çeviriyoruz
                var devices = await _httpClient.GetFromJsonAsync<List<SmartDeviceModel>>(BaseUrl);
                return devices ?? new List<SmartDeviceModel>();
            }
            catch (Exception ex)
            {
                // Eğer API kapalıysa veya hata verirse uygulama çökmesin diye boş liste dönüyoruz
                return new List<SmartDeviceModel>();
            }
        }

        // 2. Postman'deki POST İsteği: Test Cihazları Ekle
        public async Task AddTestDevicesAsync()
        {
            try
            {
                await _httpClient.PostAsync($"{BaseUrl}/add-test-devices", null);
            }
            catch { /* Hata yönetimi buraya eklenebilir */ }
        }

        // 3. Postman'deki POST İsteği: Tüm Cihazları Aç (Eve Geldim Senaryosu)
        public async Task TurnOnAllAsync()
        {
            try
            {
                await _httpClient.PostAsync($"{BaseUrl}/turn-on-all", null);
            }
            catch { /* Hata yönetimi buraya eklenebilir */ }
        }

        // 4. Postman'deki POST İsteği: Belirli bir cihazın durumunu değiştir
        public async Task ToggleDeviceAsync(Guid id)
        {
            try
            {
                // Cihazın ID'sini URL'ye ekleyerek API'ye gönderiyoruz
                await _httpClient.PostAsync($"{BaseUrl}/{id}/toggle", null);
            }
            catch { /* Hata yönetimi buraya eklenebilir */ }
        }
    }
}