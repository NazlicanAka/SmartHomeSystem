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

// 📢 Event-Driven Architecture: Event Dispatcher ve Handlers
builder.Services.AddSingleton<SmartHome.API.Application.Events.IEventDispatcher, SmartHome.API.Application.Events.EventDispatcher>();

// Event Handlers - DeviceStateChanged için tüm handler'lar

builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceStateChangedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();
// 🤖 OTOMASYON HANDLER: Robot süpürge açıldığında hava temizleyicileri kapat/aç
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceStateChangedEvent>, 
    SmartHome.API.Application.EventHandlers.AutomationRuleHandler>();

// Event Handlers - DeviceAdded için

builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceAddedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();

// Event Handlers - DeviceRemoved için

builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceRemovedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();

// Event Handlers - AutomationTriggered için

builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.AutomationTriggeredEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();

// Event Handlers - UserPresenceChanged için

builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.UserPresenceChangedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();

// Event Handlers - EnergySavingTriggered için

builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.EnergySavingTriggeredEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();

// 📡 SignalR - Gerçek zamanlı bildirimler
builder.Services.AddSignalR();

// 📡 Cihaz Protokol Adaptörlerini sisteme kaydediyoruz
builder.Services.AddTransient<SmartHome.API.Application.Interfaces.IDeviceProtocolAdapter, SmartHome.API.Infrastructure.Adapters.WiFiAdapter>();
builder.Services.AddTransient<SmartHome.API.Application.Interfaces.IDeviceProtocolAdapter, SmartHome.API.Infrastructure.Adapters.BluetoothAdapter>();

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
