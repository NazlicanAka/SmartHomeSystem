using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.WPF.Models;
using SmartHome.WPF.Services;

namespace SmartHome.WPF.ViewModels
{

    // DİKKAT: "partial" kelimesi çok önemlidir. Kurduğumuz Toolkit paketi arka planda bizim için ekstra kodlar üretecek.
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        // ObservableCollection: Normal List'ten farkı, içine eleman eklendiğinde veya silindiğinde
        // ekranın (arayüzün) otomatik olarak anında güncellenmesini (tepki vermesini) sağlar.
        [ObservableProperty]
        private ObservableCollection<SmartDeviceModel> _devices;

        // Kullanıcının arayüzden gireceği yeni cihaz bilgileri
        [ObservableProperty]
        private string _newDeviceName;

        [ObservableProperty]
        private string _newDeviceType = "Light"; // Varsayılan değer Işık olsun

        // Kullanıcının arayüzde göreceği cihaz türleri
        public List<string> DeviceTypes { get; } = new List<string>
        {
            "Light",
            "Thermostat",
            "AirPurifier",
            "RobotVacuum"
        };

        public MainViewModel()
        {
            _apiService = new ApiService();
            Devices = new ObservableCollection<SmartDeviceModel>();

            // Uygulama açıldığında cihazları getirmek yerine önce sistemi başlat (Giriş yap)
            _ = LoadDevicesAsync();
        }

        

        // [RelayCommand] etiketi sayesinde bu metotlar otomatik olarak ekrandaki butonlara (Command) dönüşür.

        [RelayCommand]
        public async Task LoadDevicesAsync()
        {
            var apiDevices = await _apiService.GetDevicesAsync();
            Devices.Clear(); // Ekranı temizle
            foreach (var device in apiDevices)
            {
                Devices.Add(device); // API'den gelenleri ekrana ekle
            }
        }


        [RelayCommand]
        public async Task AddDeviceAsync()
        {
            if (string.IsNullOrWhiteSpace(NewDeviceName)) return;

            bool isSuccess = await _apiService.AddDeviceAsync(NewDeviceName, NewDeviceType);
            if (isSuccess)
            {
                NewDeviceName = string.Empty; // Eklendikten sonra kutuyu temizle
                await LoadDevicesAsync();
            }
            else
            {
                System.Windows.MessageBox.Show("Cihaz ekleme yetkiniz yok (Sadece Ebeveynler).", "Yetki Hatası");
            }
        }

        [RelayCommand]
        public async Task DeleteDeviceAsync(SmartDeviceModel device)
        {
            if (device != null)
            {
                bool isSuccess = await _apiService.DeleteDeviceAsync(device.Id);
                if (isSuccess)
                    await LoadDevicesAsync();
                else
                    System.Windows.MessageBox.Show("Cihaz silme yetkiniz yok (Sadece Ebeveynler).", "Yetki Hatası");
            }
        }


        [RelayCommand]
        public async Task ToggleDeviceAsync(SmartDeviceModel device)
        {
            if (device != null)
            {
                // Seçilen cihazın ID'sini servise gönderiyoruz
                await _apiService.ToggleDeviceAsync(device.Id);

                // İşlem bittikten sonra ekrandaki AÇIK/KAPALI yazısının güncellenmesi için listeyi yeniliyoruz
                await LoadDevicesAsync();
            }
        }

        [RelayCommand]
        public void Logout(System.Windows.Window currentWindow)
        {
            // 1. Kuryenin hafızasını temizle
            _apiService.Logout();

            // 2. Giriş Ekranını (LoginWindow) yeniden oluştur ve göster
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            // 3. Şu anki Ana Ekranı (MainWindow) tamamen kapat
            currentWindow?.Close();
        }
    }
}