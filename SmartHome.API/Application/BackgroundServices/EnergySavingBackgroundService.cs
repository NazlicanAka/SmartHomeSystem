namespace SmartHome.API.Application.BackgroundServices;

// Enerji tasarrufu için her dakika çalışan arka plan servisi.
// Tüm açık ışıkları otomatik olarak kapatır.
public class EnergySavingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EnergySavingBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // Dakikada bir çalış

    public EnergySavingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<EnergySavingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // İlk çalışma için biraz bekle (uygulama başlatılırken)
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Scoped service kullanmak için yeni bir scope oluştur
                using (var scope = _serviceProvider.CreateScope())
                {
                    var deviceService = scope.ServiceProvider
                        .GetRequiredService<Interfaces.IDeviceService>();

                    await deviceService.TriggerEnergySavingAsync();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Enerji tasarrufu kontrolü sırasında hata oluştu.");
            }

            // Dakikada bir tekrarla
            await Task.Delay(_interval, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Enerji Tasarrufu Servisi durduruluyor...");
        return base.StopAsync(cancellationToken);
    }
}
