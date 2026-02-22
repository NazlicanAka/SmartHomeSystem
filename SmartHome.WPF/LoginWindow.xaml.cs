using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartHome.WPF
{
    /// <summary>
    /// LoginWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            var viewModel = new ViewModels.LoginViewModel();
            DataContext = viewModel;

            // PasswordBox'ın şifresini ViewModel'e bağla
            PasswordBox.PasswordChanged += (sender, e) =>
            {
                viewModel.Password = PasswordBox.Password;
            };
        }
    }
}
