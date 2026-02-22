using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SmartHome.WPF.Models;
using SmartHome.WPF.Services;

namespace SmartHome.WPF
{
    public partial class DeviceHistoryWindow : Window
    {
        private readonly ApiService _apiService;

        public DeviceHistoryWindow(List<DeviceHistoryModel> history, string deviceName = null)
        {
            InitializeComponent();
            _apiService = new ApiService();

            // Başlık ve alt bilgileri ayarla
            if (!string.IsNullOrEmpty(deviceName))
            {
                TitleTextBlock.Text = $"📊 {deviceName.ToUpper()} GEÇMİŞİ";
                SubtitleTextBlock.Text = $"Bu cihaza ait tüm işlemler";
            }
            else
            {
                TitleTextBlock.Text = "📊 TÜM CİHAZ GEÇMİŞİ";
                SubtitleTextBlock.Text = "Sistemdeki tüm cihaz işlemleri";
            }

            // DataGrid'e veriyi bağla
            HistoryDataGrid.ItemsSource = history;

            // Kayıt sayısını göster
            CountTextBlock.Text = $"Toplam {history.Count} kayıt gösteriliyor";
        }

        private async void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Tüm geçmiş kayıtları silmek istediğinize emin misiniz?\n\nBu işlem geri alınamaz!",
                "Geçmişi Temizle",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                bool success = await _apiService.ClearHistoryAsync();
                if (success)
                {
                    MessageBox.Show("✅ Tüm geçmiş kayıtları başarıyla temizlendi!", "Başarılı");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("❌ Geçmiş temizlenirken bir hata oluştu.\n\nSadece Ebeveynler bu işlemi yapabilir.", "Hata");
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
