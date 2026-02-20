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

        public MainViewModel()
        {
            _apiService = new ApiService();
            Devices = new ObservableCollection<SmartDeviceModel>();

            // Uygulama açılır açılmaz cihazları API'den çekip getirmesi için:
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
        public async Task AddTestDevicesAsync()
        {
            await _apiService.AddTestDevicesAsync(); // API'ye istek at
            await LoadDevicesAsync(); // Ekledikten sonra ekrandaki listeyi hemen yenile
        }

        [RelayCommand]
        public async Task TurnOnAllAsync()
        {
            await _apiService.TurnOnAllAsync(); // API'ye tümünü aç isteği at
            await LoadDevicesAsync(); // Durumların "AÇIK" olduğunu görmek için listeyi yenile
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
    }
}