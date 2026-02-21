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

// Cihaz Protokol Adaptörlerimizi sisteme kaydediyoruz
builder.Services.AddTransient<SmartHome.API.Application.Interfaces.IDeviceProtocolAdapter, SmartHome.API.Infrastructure.Adapters.WiFiAdapter>();
builder.Services.AddTransient<SmartHome.API.Application.Interfaces.IDeviceProtocolAdapter, SmartHome.API.Infrastructure.Adapters.BluetoothAdapter>();
// Arka plan işçimizi sisteme dahil ediyoruz
builder.Services.AddHostedService<SmartHome.API.Application.Services.EnergySavingBackgroundService>();

builder.Services.AddDbContext<SmartHome.API.Infrastructure.Data.SmartHomeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
