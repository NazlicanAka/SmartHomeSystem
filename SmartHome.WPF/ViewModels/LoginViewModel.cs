using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.WPF.Services;
using System.Windows;

namespace SmartHome.WPF.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        public LoginViewModel()
        {
            _apiService = new ApiService();
        }

        [RelayCommand]
        public async Task LoginAsync(Window currentWindow)
        {
            // Kullanıcının ekrana yazdığı bilgilerle API'ye gidiyoruz
            bool isApprove = await _apiService.LoginAsync(Username, Password);

            if (isApprove)
            {
                // Giriş başarılıysa Ana Ekranı aç ve bu Giriş Ekranını kapat
                var mainWindow = new MainWindow();
                mainWindow.Show();
                currentWindow.Close();
            }
            else
            {
                MessageBox.Show("Hatalı kullanıcı adı veya şifre!", "Giriş Başarısız", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        public async Task RegisterParentAsync() => await RegisterAsync("Parent");

        [RelayCommand]
        public async Task RegisterChildAsync() => await RegisterAsync("Child");

        private async Task RegisterAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Kayıt olmak için lütfen kullanıcı adı ve şifre yazın!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isSuccess = await _apiService.RegisterAsync(Username, Password, role);
            if (isSuccess)
                MessageBox.Show($"{role} rolüyle başarıyla kayıt oldunuz! Şimdi 'Sisteme Giriş Yap' butonunu kullanabilirsiniz.", "Başarılı");
            else
                MessageBox.Show("Kayıt başarısız. Bu kullanıcı adı zaten alınmış olabilir.", "Hata");
        }
    }
}