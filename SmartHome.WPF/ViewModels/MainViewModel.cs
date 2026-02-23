using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.WPF.Models;
using SmartHome.WPF.Services;
using System.Windows;

namespace SmartHome.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ApiService _apiService; // REST API ile iletişim kuran servisimiz
        private readonly SignalRService _signalRService; // haberi dinleyen telsiz görevinde.

        // ObservableCollection: Normal List'ten farkı, içine eleman eklendiğinde veya silindiğinde
        // ekranın (arayüzün) otomatik olarak anında güncellenmesini (tepki vermesini) sağlar.
        [ObservableProperty]
        private ObservableCollection<SmartDeviceModel> _devices;

        // Kullanıcının arayüzden gireceği yeni cihaz bilgileri
        [ObservableProperty]
        private string _newDeviceName;

        [ObservableProperty]
        private string _newDeviceType = "Light";

        [ObservableProperty]
        private string _selectedProtocol = "Wi-Fi";

        [ObservableProperty]
        private bool _isHomeNetworkConnected;

        [ObservableProperty]
        private string _connectionStatus = "Bağlanıyor...";

        [ObservableProperty]
        private bool _isConnected;

        // Kullanıcının arayüzde göreceği cihaz türleri (Backend'den dinamik olarak yüklenir)
        [ObservableProperty]
        private ObservableCollection<string> _deviceTypes = new();

        // Desteklenen protokoller (Backend'den dinamik olarak yüklenir)
        [ObservableProperty]
        private ObservableCollection<string> _protocols = new();

        public MainViewModel()
        {
            _apiService = new ApiService();
            _signalRService = new SignalRService();
            Devices = new ObservableCollection<SmartDeviceModel>();
            DeviceTypes = new ObservableCollection<string>();
            Protocols = new ObservableCollection<string>();

            // SignalR event'lerini dinle
            InitializeSignalREvents();

            // Backend'den cihaz türleri ve protokolleri yükle
            _ = LoadConfigurationAsync();

            // Uygulama açıldığında cihazları getir
            _ = LoadDevicesAsync();

            // SignalR bağlantısını başlat
            _ = ConnectSignalRAsync();
        }

        // Backend'den cihaz türleri ve protokolleri yükle
        private async Task LoadConfigurationAsync()
        {
            var deviceTypes = await _apiService.GetDeviceTypesAsync();
            var protocols = await _apiService.GetProtocolsAsync();

            DeviceTypes.Clear();
            Protocols.Clear();

            foreach (var type in deviceTypes)
                DeviceTypes.Add(type);

            foreach (var protocol in protocols)
                Protocols.Add(protocol);

            // Varsayılan değerleri ayarla
            if (DeviceTypes.Any())
                NewDeviceType = DeviceTypes.First();

            if (Protocols.Any())
                SelectedProtocol = Protocols.First();
        }

        private void InitializeSignalREvents()
        {
            // Bağlantı durumu değişikliklerini izle
            _signalRService.ConnectionStateChanged += (sender, state) =>
            {
                Application.Current.Dispatcher.Invoke(() => // ekranla konuşma izni.
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
                });
            };

            // Yeni cihaz eklendiğinde
            _signalRService.DeviceAdded += async (sender, args) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadDevicesAsync();
                });
            };

            // Cihaz silindiğinde
            _signalRService.DeviceRemoved += async (sender, args) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadDevicesAsync();
                });
            };

            // Otomasyon tetiklendiğinde
            _signalRService.AutomationTriggered += async (sender, message) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadDevicesAsync();
                });
            };

            // Kullanıcı presence değiştiğinde
            _signalRService.UserPresenceChanged += async (sender, message) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadDevicesAsync();
                });
            };

            // Enerji tasarrufu tetiklendiğinde
            _signalRService.EnergySavingTriggered += async (sender, message) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadDevicesAsync();
                });
            };
        }

        private async Task ConnectSignalRAsync()
        {
            await _signalRService.StartAsync();
        }

        [RelayCommand]
        public async Task LoadDevicesAsync()
        {
            var apiDevices = await _apiService.GetDevicesAsync();
            Devices.Clear();
            foreach (var device in apiDevices)
            {
                Devices.Add(device);
            }
        }


        [RelayCommand]
        public async Task AddDeviceAsync()
        {
            if (string.IsNullOrWhiteSpace(NewDeviceName)) return;

            bool isSuccess = await _apiService.AddDeviceAsync(NewDeviceName, NewDeviceType, SelectedProtocol);
            if (isSuccess)
            {
                NewDeviceName = string.Empty;
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
            _apiService.Logout();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
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