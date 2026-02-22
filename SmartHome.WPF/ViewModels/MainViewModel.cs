using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.WPF.Models;
using SmartHome.WPF.Services;
using System.Windows;

namespace SmartHome.WPF.ViewModels
{

    // DİKKAT: "partial" kelimesi çok önemlidir. Kurduğumuz Toolkit paketi arka planda bizim için ekstra kodlar üretecek.
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private readonly SignalRService _signalRService;

        // ObservableCollection: Normal List'ten farkı, içine eleman eklendiğinde veya silindiğinde
        // ekranın (arayüzün) otomatik olarak anında güncellenmesini (tepki vermesini) sağlar.
        [ObservableProperty]
        private ObservableCollection<SmartDeviceModel> _devices;

        // Kullanıcının arayüzden gireceği yeni cihaz bilgileri
        [ObservableProperty]
        private string _newDeviceName;

        [ObservableProperty]
        private string _newDeviceType = "Light"; // Varsayılan değer Işık olsun

        [ObservableProperty]
        private string _selectedProtocol = "Wi-Fi";

        [ObservableProperty]
        private bool _isHomeNetworkConnected;

        [ObservableProperty]
        private string _connectionStatus = "Bağlanıyor...";

        [ObservableProperty]
        private bool _isConnected;

        // Kullanıcının arayüzde göreceği cihaz türleri
        public List<string> DeviceTypes { get; } = new List<string>
        {
            "Light",
            "Thermostat",
            "AirPurifier",
            "RobotVacuum"
        };

        public List<string> Protocols { get; } = new List<string> { "Wi-Fi", "Bluetooth" };

        public MainViewModel()
        {
            _apiService = new ApiService();
            _signalRService = new SignalRService();
            Devices = new ObservableCollection<SmartDeviceModel>();

            // SignalR event'lerini dinle
            InitializeSignalREvents();

            // Uygulama açıldığında cihazları getirmek yerine önce sistemi başlat (Giriş yap)
            _ = LoadDevicesAsync();

            // SignalR bağlantısını başlat
            _ = ConnectSignalRAsync();
        }

        private void InitializeSignalREvents()
        {
            // Bağlantı durumu değişikliklerini izle
            _signalRService.ConnectionStateChanged += (sender, state) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ConnectionStatus = state switch
                    {
                        ConnectionState.Connected => "🟢 Bağlı (Gerçek Zamanlı)",
                        ConnectionState.Connecting => "🟡 Bağlanıyor...",
                        ConnectionState.Reconnecting => "🟡 Yeniden bağlanıyor...",
                        ConnectionState.Disconnected => "🔴 Bağlantı Kesildi",
                        ConnectionState.Failed => "❌ Bağlantı Hatası",
                        _ => "⚪ Bilinmeyen"
                    };
                    IsConnected = state == ConnectionState.Connected;
                });
            };

            // Cihaz durumu değiştiğinde
            _signalRService.DeviceStateChanged += async (sender, args) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    // Listeyi yenile
                    await LoadDevicesAsync();

                    // Bildirim göster
                    ShowNotification($"📱 {args.DeviceName}", 
                        $"{(args.IsOn ? "Açıldı" : "Kapatıldı")} - {args.ChangedBy}");
                });
            };

            // Yeni cihaz eklendiğinde
            _signalRService.DeviceAdded += async (sender, args) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadDevicesAsync();
                    ShowNotification("➕ Yeni Cihaz Eklendi", args.Message);
                });
            };

            // Cihaz silindiğinde
            _signalRService.DeviceRemoved += async (sender, args) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadDevicesAsync();
                    ShowNotification("🗑️ Cihaz Silindi", args.Message);
                });
            };

            // Otomasyon tetiklendiğinde
            _signalRService.AutomationTriggered += async (sender, message) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadDevicesAsync();
                    ShowNotification("🤖 Otomasyon Çalıştı", message);
                });
            };

            // Kullanıcı presence değiştiğinde
            _signalRService.UserPresenceChanged += async (sender, message) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadDevicesAsync();
                    ShowNotification("🏠 Presence Değişti", message);
                });
            };
        }

        private async Task ConnectSignalRAsync()
        {
            await _signalRService.StartAsync();
        }

        private void ShowNotification(string title, string message)
        {
            // Toast notification göster (basit MessageBox ile)
            // Production'da Windows Toast Notification kullanılabilir
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
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

            bool isSuccess = await _apiService.AddDeviceAsync(NewDeviceName, NewDeviceType, SelectedProtocol);
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

        [RelayCommand]
        public async Task TogglePresenceAsync()
        {
            try
            {
                bool isSuccess = await _apiService.TriggerPresenceAsync(IsHomeNetworkConnected);
                if (isSuccess)
                {
                    await LoadDevicesAsync();
                }
                else
                {
                    System.Windows.MessageBox.Show("Varlık durumu değiştirilemedi.", "Hata");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata");
            }
        }

        [RelayCommand]
        public async Task ShowDeviceHistoryAsync()
        {
            var history = await _apiService.GetDeviceHistoryAsync();
            if (history.Any())
            {
                var window = new DeviceHistoryWindow(history);
                window.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("Henüz kayıtlı geçmiş yok.", "Bilgi");
            }
        }

        [RelayCommand]
        public async Task ShowSpecificDeviceHistoryAsync(SmartDeviceModel device)
        {
            if (device != null)
            {
                var history = await _apiService.GetDeviceHistoryAsync(device.Id);
                if (history.Any())
                {
                    var window = new DeviceHistoryWindow(history, device.Name);
                    window.ShowDialog();
                }
                else
                {
                    System.Windows.MessageBox.Show($"{device.Name} için henüz kayıt yok.", "Bilgi");
                }
            }
        }
    }
}