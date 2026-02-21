using SmartHome.API.Application.Interfaces;
using SmartHome.API.Domain.Enums;

namespace SmartHome.API.Application.Services
{
    // BackgroundService'den miras alarak bunun arka planda sürekli çalışacak bir işçi olduğunu söylüyoruz.
    public class EnergySavingBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public EnergySavingBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Uygulama çalıştığı sürece bu döngü sonsuza kadar döner
            while (!stoppingToken.IsCancellationRequested)
            {
                // Test için 1 dakika bekliyoruz. (Gerçekte TimeSpan.FromHours(1) falan olur)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                // MÜLAKAT NOTU: BackgroundService "Singleton" (tek kopya) çalışır.
                // Bizim veritabanımız ve DeviceService'imiz ise "Scoped" (istek başına).
                // Singleton içinden Scoped çağıramayacağımız için, burada kendi Scope'umuzu (Kapsamımızı) yaratıyoruz!
                using (var scope = _serviceProvider.CreateScope())
                {
                    var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
                    var devices = deviceService.GetAllDevices();

                    foreach (var device in devices)
                    {
                        // Sadece IŞIK tipindeki cihazları bul
                        if (device.Type == DeviceType.Light && device.IsOn)
                        {
                            // Açık unutulmuş ışığı kapat!
                            deviceService.ToggleDevice(device.Id);

                            // Not: Gerçek bir sistemde burada loglama (Console.WriteLine vb.) yapılır.
                        }
                    }
                }
            }
        }
    }
}