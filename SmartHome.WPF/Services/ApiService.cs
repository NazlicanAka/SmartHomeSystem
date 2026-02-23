using System.Net.Http;
using System.Net.Http.Json; // JSON verilerini otomatik okumak için
using SmartHome.WPF.Models;
using System.Net.Http.Headers;

namespace SmartHome.WPF.Services
{
    public class ApiService
    {
        // Static yaparak biletin tüm uygulama boyunca hatırlanmasını sağlıyoruz
        private static readonly HttpClient _httpClient = new HttpClient();
        private static string _jwtToken;

        // Buradaki port numarasını (7106), kendi siyah ekranında yazan numarayla değiştir!
        private const string BaseUrl = "https://localhost:7106/api/devices";

        // AuthController'a ulaşıp giriş yapmak için kullanacağımız tam adres
        private const string AuthUrl = "https://localhost:7106/api/auth/login";
        private const string RegisterUrl = "https://localhost:7106/api/auth/register";

        // Yeni Eklenen Giriş Yapma Metodu
        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                // Kullanıcı adı ve şifreyi JSON olarak API'ye gönderiyoruz
                var loginData = new { Username = username, Password = password };
                var response = await _httpClient.PostAsJsonAsync(AuthUrl, loginData);

                if (response.IsSuccessStatusCode)
                {
                    // API'den dönen Token'ı okuyoruz
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    _jwtToken = result.Token;

                    // EN ÖNEMLİ KISIM: Bundan sonraki tüm API isteklerinin başlığına (Header) bu bileti ekliyoruz!
                    // Güvenlik görevlisi bu "Bearer" (Taşıyıcı) kelimesini arar.
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string username, string password, string role)
        {
            try
            {
                var registerData = new { Username = username, Password = password, Role = role };
                var response = await _httpClient.PostAsJsonAsync(RegisterUrl, registerData);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // API'den gelen veriyi karşılamak için küçük bir şablon sınıf
        public class LoginResponse
        {
            public string Token { get; set; }
        }

        public async Task<bool> AddDeviceAsync(string name, string type, string protocol)
        {
            var dto = new { Name = name, Type = type, Protocol = protocol };
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDeviceAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
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

        // Çıkış yapıldığında hafızadaki bileti temizliyoruz
        public void Logout()
        {
            _jwtToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<bool> TriggerPresenceAsync(bool isHome)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/presence?isHome={isHome}", new { });
            return response.IsSuccessStatusCode;
        }

        // 📊 Cihaz geçmişini getir
        public async Task<List<DeviceHistoryModel>> GetDeviceHistoryAsync(Guid? deviceId = null)
        {
            try
            {
                string url = $"{BaseUrl}/history";
                if (deviceId.HasValue)
                {
                    url += $"?deviceId={deviceId.Value}";
                }

                var history = await _httpClient.GetFromJsonAsync<List<DeviceHistoryModel>>(url);
                return history ?? new List<DeviceHistoryModel>();
            }
            catch
            {
                return new List<DeviceHistoryModel>();
            }
        }

        // 🗑️ Tüm geçmişi temizle
        public async Task<bool> ClearHistoryAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/history/clear");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // 📋 Desteklenen cihaz türlerini getir
        public async Task<List<string>> GetDeviceTypesAsync()
        {
            try
            {
                var deviceTypes = await _httpClient.GetFromJsonAsync<List<string>>($"{BaseUrl}/types");
                return deviceTypes ?? new List<string>();
            }
            catch
            {
                // Fallback: Backend'den alamazsak varsayılan liste
                return new List<string> { "Light", "Thermostat", "AirPurifier", "RobotVacuum" };
            }
        }

        // 📋 Desteklenen protokolleri getir
        public async Task<List<string>> GetProtocolsAsync()
        {
            try
            {
                var protocols = await _httpClient.GetFromJsonAsync<List<string>>($"{BaseUrl}/protocols");
                return protocols ?? new List<string>();
            }
            catch
            {
                // Fallback: Backend'den alamazsak varsayılan liste
                return new List<string> { "Wi-Fi", "Bluetooth", "Zigbee" };
            }
        }
    }


}