using CommunityToolkit.Mvvm.Input;
using SmartHome.WPF.Models;
using SmartHome.WPF.Services;
using SmartHome.WPF.ViewModels; // ViewModel'imizi kullanabilmek için ekledik
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartHome.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // İşte MVVM'in sırrı! Bu ekranın veri yöneticisi MainViewModel'dir diyoruz.
            DataContext = new MainViewModel();
        }

    }
}