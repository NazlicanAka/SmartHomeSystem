using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// JWT Kimlik Doğrulama Ayarları
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Artık veritabanı kullanacağımız için servisimizi Scoped (istek başına) olarak değiştiriyoruz.
builder.Services.AddScoped<SmartHome.API.Application.Interfaces.IDeviceService, SmartHome.API.Application.Services.DeviceService>();

// 🔧 Factory Pattern: Adapter seçimi için
builder.Services.AddSingleton<SmartHome.API.Application.Factories.IDeviceAdapterFactory, SmartHome.API.Application.Factories.DeviceAdapterFactory>();

// 📢 Event-Driven Architecture: Event Dispatcher ve Handlers
builder.Services.AddSingleton<SmartHome.API.Application.Events.IEventDispatcher, SmartHome.API.Application.Events.EventDispatcher>();

// Event Handlers - DeviceStateChanged için tüm handler'lar
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceStateChangedEvent>, 
    SmartHome.API.Application.EventHandlers.DeviceStateChangedLoggingHandler>();
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceStateChangedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();

// Event Handlers - DeviceAdded için
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceAddedEvent>, 
    SmartHome.API.Application.EventHandlers.DeviceAddedNotificationHandler>();
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceAddedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();

// Event Handlers - DeviceRemoved için
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceRemovedEvent>, 
    SmartHome.API.Application.EventHandlers.DeviceRemovedNotificationHandler>();
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceRemovedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();

// Event Handlers - AutomationTriggered için
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.AutomationTriggeredEvent>, 
    SmartHome.API.Application.EventHandlers.AutomationTriggeredNotificationHandler>();
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.AutomationTriggeredEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();

// Event Handlers - UserPresenceChanged için
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.UserPresenceChangedEvent>, 
    SmartHome.API.Application.EventHandlers.UserPresenceChangedNotificationHandler>();
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.UserPresenceChangedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();

// 📡 SignalR - Gerçek zamanlı bildirimler
builder.Services.AddSignalR();

// 📡 Cihaz Protokol Adaptörlerini sisteme kaydediyoruz (Eski + Yeni)
// Basic Adapters (mevcut)
builder.Services.AddTransient<SmartHome.API.Application.Interfaces.IDeviceProtocolAdapter, SmartHome.API.Infrastructure.Adapters.WiFiAdapter>();
builder.Services.AddTransient<SmartHome.API.Application.Interfaces.IDeviceProtocolAdapter, SmartHome.API.Infrastructure.Adapters.BluetoothAdapter>();

// Advanced Adapters (gelişmiş özelliklerle)
builder.Services.AddTransient<SmartHome.API.Application.Interfaces.IDeviceProtocolAdapter, SmartHome.API.Infrastructure.Adapters.AdvancedWiFiAdapter>();
builder.Services.AddTransient<SmartHome.API.Application.Interfaces.IDeviceProtocolAdapter, SmartHome.API.Infrastructure.Adapters.AdvancedBluetoothAdapter>();
builder.Services.AddTransient<SmartHome.API.Application.Interfaces.IDeviceProtocolAdapter, SmartHome.API.Infrastructure.Adapters.ZigbeeAdapter>();

// Arka plan işçimizi sisteme dahil ediyoruz
builder.Services.AddHostedService<SmartHome.API.Application.Services.EnergySavingBackgroundService>();

builder.Services.AddDbContext<SmartHome.API.Infrastructure.Data.SmartHomeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

// 🔧 Veritabanını otomatik güncelle (Migration'ları uygula)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SmartHome.API.Infrastructure.Data.SmartHomeDbContext>();
    dbContext.Database.Migrate(); // Bekleyen tüm migration'ları uygular
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 📢 SignalR Hub'ı map et
app.MapHub<SmartHome.API.Hubs.DeviceNotificationHub>("/hubs/notifications");

app.Run();
