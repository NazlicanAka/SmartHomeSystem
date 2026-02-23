### SMART HOME API - SWAGGER ENTEGRASYON KODU

Bu dosya Program.cs'e eklenecek Swagger yapılandırmasını içerir.

#### 1. NuGet Package Yükleme

```bash
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
```

#### 2. Program.cs Güncellemesi

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // API Bilgileri
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Smart Home Management API",
        Description = "IoT tabanlı akıllı ev cihazlarını yönetmek için RESTful API. " +
                      "SignalR ile gerçek zamanlı iletişim, JWT ile güvenli authentication, " +
                      "Event-Driven Architecture ve Clean Architecture prensipleri uygulanmıştır.",
        Contact = new OpenApiContact
        {
            Name = "Nazlican Aka",
            Email = "nazlican@example.com",
            Url = new Uri("https://github.com/NazlicanAka/SmartHomeSystem")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    
    // JWT Bearer Authentication için Swagger Security Definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header kullanarak Bearer token girin.
                      
Adımlar:
1. /api/auth/login endpoint'ini kullanarak giriş yapın
2. Dönen token değerini kopyalayın
3. Aşağıdaki alana 'Bearer {token}' formatında girin
                      
Örnek: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    
    // XML Documentation (opsiyonel - detaylı açıklamalar için)
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
    
    // Endpoint'leri grupla
    options.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return new[] { api.GroupName };
        }
        
        var controllerActionDescriptor = api.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (controllerActionDescriptor != null)
        {
            return new[] { controllerActionDescriptor.ControllerName };
        }
        
        throw new InvalidOperationException("Unable to determine tag for endpoint.");
    });
    
    options.DocInclusionPredicate((name, api) => true);
});

// JWT Authentication
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

// Diğer servisler...
builder.Services.AddScoped<SmartHome.API.Application.Interfaces.IDeviceService, SmartHome.API.Application.Services.DeviceService>();
builder.Services.AddSingleton<SmartHome.API.Application.Events.IEventDispatcher, SmartHome.API.Application.Events.EventDispatcher>();

// Event Handlers
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceStateChangedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceStateChangedEvent>, 
    SmartHome.API.Application.EventHandlers.AutomationRuleHandler>();
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceAddedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.DeviceRemovedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.AutomationTriggeredEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.UserPresenceChangedEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();
builder.Services.AddTransient<SmartHome.API.Application.Events.IEventHandler<SmartHome.API.Domain.Events.EnergySavingTriggeredEvent>, 
    SmartHome.API.Application.EventHandlers.SignalRNotificationHandler>();

// SignalR
builder.Services.AddSignalR();

// Protocol Adapters
builder.Services.AddTransient<SmartHome.API.Application.Interfaces.IDeviceProtocolAdapter, SmartHome.API.Infrastructure.Adapters.WiFiAdapter>();
builder.Services.AddTransient<SmartHome.API.Application.Interfaces.IDeviceProtocolAdapter, SmartHome.API.Infrastructure.Adapters.BluetoothAdapter>();

// Database
builder.Services.AddDbContext<SmartHome.API.Infrastructure.Data.SmartHomeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SmartHome.API.Infrastructure.Data.SmartHomeDbContext>();
    dbContext.Database.Migrate();
}

// Swagger Middleware (Development ve Production'da)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Home API v1");
    options.RoutePrefix = string.Empty; // Swagger UI root URL'de olsun (https://localhost:7106/)
    options.DocumentTitle = "Smart Home API - Swagger UI";
    
    // UI Customization
    options.DefaultModelsExpandDepth(-1); // Model'leri gizle (daha temiz görünüm)
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Endpoint'leri daraltılmış göster
    options.EnableDeepLinking(); // URL'den direkt endpoint'e gidebilme
    options.DisplayOperationId(); // Operation ID'leri göster
    options.EnableFilter(); // Arama filtresi
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<SmartHome.API.Hubs.DeviceNotificationHub>("/hubs/notifications");

app.Run();
```

#### 3. XML Documentation Aktif Etme (Opsiyonel)

SmartHome.API.csproj dosyasına ekleyin:

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <!-- XML Documentation -->
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

#### 4. Controller'lara XML Comments Ekleme

```csharp
/// <summary>
/// Akıllı ev cihazlarını yönetmek için endpoint'ler
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DevicesController : ControllerBase
{
    /// <summary>
    /// Tüm cihazları listeler
    /// </summary>
    /// <returns>Cihaz listesi</returns>
    /// <response code="200">Başarılı - Cihaz listesi döndürüldü</response>
    /// <response code="401">Yetkisiz - JWT token gerekli</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SmartDeviceModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetAllDevices()
    {
        var devices = _deviceService.GetAllDevices();
        return Ok(devices);
    }
    
    /// <summary>
    /// Yeni cihaz ekler (Sadece Parent rolü)
    /// </summary>
    /// <param name="dto">Cihaz bilgileri</param>
    /// <returns>İşlem sonucu</returns>
    /// <response code="200">Başarılı - Cihaz eklendi</response>
    /// <response code="400">Hatalı istek - Geçersiz cihaz türü</response>
    /// <response code="401">Yetkisiz - JWT token gerekli</response>
    /// <response code="403">Yasak - Sadece Parent kullanıcılar cihaz ekleyebilir</response>
    [HttpPost]
    [Authorize(Roles = "Parent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddDevice([FromBody] AddDeviceDto dto)
    {
        if (Enum.TryParse<DeviceType>(dto.Type, out var deviceType))
        {
            var username = User.Identity?.Name ?? "Bilinmeyen";
            await _deviceService.AddCustomDeviceAsync(dto.Name, deviceType, dto.Protocol, username);
            return Ok(new { message = "Cihaz başarıyla eklendi" });
        }
        return BadRequest(new { message = "Geçersiz cihaz türü" });
    }
    
    /// <summary>
    /// Cihazı aç/kapat
    /// </summary>
    /// <param name="id">Cihaz ID'si</param>
    /// <returns>İşlem sonucu</returns>
    /// <response code="200">Başarılı - Cihaz durumu değiştirildi</response>
    /// <response code="401">Yetkisiz - JWT token gerekli</response>
    /// <response code="404">Bulunamadı - Cihaz ID'si geçersiz</response>
    [HttpPost("{id}/toggle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleDevice(Guid id)
    {
        var username = User.Identity?.Name ?? "Bilinmeyen";
        await _deviceService.ToggleDeviceAsync(id, username);
        return Ok(new { message = "Cihaz durumu değiştirildi" });
    }
}
```

#### 5. Swagger UI Erişim

API'yi çalıştırdıktan sonra:

```
https://localhost:7106/
```

veya

```
https://localhost:7106/swagger
```

#### 6. Swagger JSON Export

```
https://localhost:7106/swagger/v1/swagger.json
```

Bu JSON'ı Postman'e import edebilir veya API dokümantasyonu için kullanabilirsiniz.

---

### Swagger UI Kullanımı:

1. **Authorize Butonuna Tıklayın** (sağ üstte)
2. **Login endpoint'ini kullanarak token alın:**
   - POST /api/auth/login
   - Username: "baba", Password: "123"
   - Token'ı kopyalayın
3. **Authorize dialog'una token'ı yapıştırın:**
   - Format: `Bearer {token}`
   - Örnek: `Bearer eyJhbGciOiJI...`
4. **Artık tüm endpoint'leri test edebilirsiniz!**

---

### Örnek Swagger Request/Response:

**POST /api/devices**

Request:
```json
{
  "name": "Yatak Odası Lambası",
  "type": "Light",
  "protocol": "WiFi"
}
```

Response (200):
```json
{
  "message": "Cihaz başarıyla eklendi"
}
```

---

### Swagger UI Özellikleri:

✅ Tüm endpoint'leri görüntüleme
✅ Request/Response örnekleri
✅ Try it out! özelliği ile direkt test
✅ JWT Authentication desteği
✅ Model schema'ları
✅ HTTP status code açıklamaları
✅ cURL command generator
✅ JSON/XML response format desteği

---

Bu yapılandırma ile API'niz **profesyonel bir dokümantasyona** sahip olacak! 🚀
