# 🏠 SMART HOME SYSTEM - KAPSAMLI PROJE DOKÜMANTASYONU

## 📋 İÇİNDEKİLER

1. [Proje Genel Bakış](#1-proje-genel-bakış)
2. [Mimari ve Tasarım Desenleri](#2-mimari-ve-tasarım-desenleri)
3. [OOP (Nesne Yönelimli Programlama) Yapıları](#3-oop-yapıları)
4. [SOLID Prensipleri Uygulaması](#4-solid-prensipleri)
5. [Katmanlı Mimari (Layered Architecture)](#5-katmanlı-mimari)
6. [Domain Driven Design (DDD)](#6-domain-driven-design)
7. [Event-Driven Architecture (EDA)](#7-event-driven-architecture)
8. [SignalR - Gerçek Zamanlı İletişim](#8-signalr-gerçek-zamanlı-iletişim)
9. [API Dokümantasyonu](#9-api-dokümantasyonu)
10. [Frontend-Backend Haberleşmesi](#10-frontend-backend-haberleşmesi)
11. [Class ve Interface Detayları](#11-class-ve-interface-detayları)
12. [Veritabanı ve Entity Framework Core](#12-veritabanı-ve-entity-framework-core)
13. [Authentication & Authorization](#13-authentication--authorization)
14. [Test Senaryoları](#14-test-senaryoları)
15. [Deployment ve Çalıştırma](#15-deployment-ve-çalıştırma)

---

## 1. PROJE GENEL BAKIŞ

### 📌 **Proje Adı:** Smart Home Management System

### 🎯 **Amaç:**
IoT tabanlı akıllı ev cihazlarını merkezi bir sistemden yönetmek, gerçek zamanlı izlemek ve otomatik senaryolar oluşturmak.

### 🛠️ **Teknoloji Stack:**

#### **Backend:**
- **.NET 10** (ASP.NET Core Web API)
- **Entity Framework Core** (ORM)
- **SQLite** (Veritabanı)
- **SignalR** (WebSocket - Gerçek Zamanlı İletişim)
- **JWT** (Authentication)

#### **Frontend:**
- **WPF** (.NET 10)
- **MVVM** (Model-View-ViewModel Pattern)
- **CommunityToolkit.Mvvm** (MVVM Framework)

#### **Mimari Desenler:**
- Layered Architecture (Katmanlı Mimari)
- Domain Driven Design (DDD)
- Event-Driven Architecture (EDA)
- Repository Pattern
- Dependency Injection (DI)
- Adapter Pattern

---

## 2. MİMARİ VE TASARIM DESENLERİ

### 📐 **Genel Mimari Diyagram**

```
┌─────────────────────────────────────────────────────────────┐
│                     WPF CLIENT (MVVM)                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │  Views       │  │  ViewModels  │  │  Models      │     │
│  │  (XAML)      │←→│  (Logic)     │←→│  (Data)      │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│         ↑                  ↑                                │
│         │                  │                                │
│  ┌──────┴──────────────────┴───────┐                       │
│  │     Services Layer               │                       │
│  │  - ApiService (HTTP)             │                       │
│  │  - SignalRService (WebSocket)    │                       │
│  └──────────────────────────────────┘                       │
└─────────────────────────────────────────────────────────────┘
                          │
                   HTTP / WebSocket
                          │
┌─────────────────────────────────────────────────────────────┐
│                   ASP.NET CORE WEB API                      │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              Controllers Layer                       │  │
│  │  - DevicesController                                 │  │
│  │  - AuthController                                    │  │
│  └──────────────────────────────────────────────────────┘  │
│                          ↓                                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │           Application Layer (Business Logic)         │  │
│  │  - Services (DeviceService)                          │  │
│  │  - Event Handlers (AutomationRuleHandler)            │  │
│  │  - Event Dispatcher (EventDispatcher)                │  │
│  └──────────────────────────────────────────────────────┘  │
│                          ↓                                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              Domain Layer (Business Rules)           │  │
│  │  - Entities (SmartLight, SmartThermostat, ...)       │  │
│  │  - Interfaces (ISmartDevice, IDeviceProtocolAdapter) │  │
│  │  - Events (DeviceStateChangedEvent, ...)             │  │
│  │  - Enums (DeviceType, Protocol)                      │  │
│  └──────────────────────────────────────────────────────┘  │
│                          ↓                                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │          Infrastructure Layer (Data Access)          │  │
│  │  - DbContext (SmartHomeDbContext)                    │  │
│  │  - Entities (DeviceEntity, UserEntity)               │  │
│  │  - Adapters (WiFiAdapter, BluetoothAdapter)          │  │
│  │  - Hubs (DeviceNotificationHub - SignalR)            │  │
│  └──────────────────────────────────────────────────────┘  │
│                          ↓                                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │                SQLite Database                       │  │
│  │  - Devices Table                                     │  │
│  │  - Users Table                                       │  │
│  │  - DeviceHistory Table                               │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### 🎯 **Kullanılan Tasarım Desenleri:**

#### **1. MVVM Pattern (Frontend)**
- **Model:** Veri yapıları (SmartDeviceModel, DeviceHistoryModel)
- **View:** XAML arayüz dosyaları (MainWindow.xaml, LoginWindow.xaml)
- **ViewModel:** UI logic (MainViewModel, LoginViewModel)

#### **2. Repository Pattern**
- Entity Framework Core ile DbContext üzerinden veri erişimi
- SmartHomeDbContext: Tüm tablolara merkezi erişim

#### **3. Dependency Injection (DI)**
- ASP.NET Core built-in DI Container
- Constructor Injection ile bağımlılık yönetimi

#### **4. Adapter Pattern**
- IDeviceProtocolAdapter: WiFi, Bluetooth adaptörleri
- Farklı protokolleri tek bir interface üzerinden yönetim

#### **5. Event-Driven Architecture**
- EventDispatcher: Event yayınlama ve dinleme
- IEventHandler: Event'leri işleyen handler'lar

#### **6. Observer Pattern**
- SignalR Hub: Backend'den frontend'e otomatik bildirim
- Event Handlers: Domain event'leri dinleme

---

## 3. OOP YAPILARI

### 🧩 **Encapsulation (Kapsülleme)**

```csharp
// Domain/Devices/SmartThermostat.cs
public class SmartThermostat : ISmartDevice
{
    // Private fields (gizli)
    private bool _isOn;
    
    // Public properties (kontrollü erişim)
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DeviceType Type => DeviceType.Thermostat;
    public bool IsOn 
    { 
        get => _isOn;
        private set => _isOn = value; // Sadece sınıf içinden set edilebilir
    }
    
    // Public methods (dışarıya açık davranışlar)
    public void TurnOn() => IsOn = true;
    public void TurnOff() => IsOn = false;
}
```

**Faydası:**
- ✅ İç detaylar gizli
- ✅ Veri bütünlüğü korunuyor
- ✅ Kontrollü erişim

---

### 🔄 **Inheritance (Kalıtım)**

```csharp
// Tüm cihazlar ISmartDevice interface'ini implement ediyor

// Domain/Interfaces/ISmartDevice.cs
public interface ISmartDevice
{
    Guid Id { get; set; }
    string Name { get; set; }
    DeviceType Type { get; }
    bool IsOn { get; set; }
    
    void TurnOn();
    void TurnOff();
    string GetStatus();
}

// Domain/Devices/SmartLight.cs
public class SmartLight : ISmartDevice
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DeviceType Type => DeviceType.Light;
    public bool IsOn { get; set; }
    
    public void TurnOn() => IsOn = true;
    public void TurnOff() => IsOn = false;
    public string GetStatus() => IsOn ? "Işık açık" : "Işık kapalı";
}

// Domain/Devices/SmartAirPurifier.cs
public class SmartAirPurifier : ISmartDevice
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DeviceType Type => DeviceType.AirPurifier;
    public bool IsOn { get; set; }
    
    public void TurnOn() => IsOn = true;
    public void TurnOff() => IsOn = false;
    public string GetStatus() => IsOn ? "Hava temizleyici çalışıyor" : "Hava temizleyici kapalı";
}
```

**Faydası:**
- ✅ Ortak davranışlar merkezi yerde
- ✅ Kod tekrarı önleniyor
- ✅ Polimorfizm ile esnek yapı

---

### 🎭 **Polymorphism (Çok Biçimlilik)**

```csharp
// Application/Services/DeviceService.cs
public IEnumerable<ISmartDevice> GetAllDevices()
{
    var entities = _context.Devices.ToList();
    var devices = new List<ISmartDevice>();
    
    foreach (var entity in entities)
    {
        // Polimorfizm: Aynı interface farklı tiplerle kullanılıyor
        if (entity.Type == DeviceType.Light)
            devices.Add(new SmartLight(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
        else if (entity.Type == DeviceType.Thermostat)
            devices.Add(new SmartThermostat(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
        else if (entity.Type == DeviceType.AirPurifier)
            devices.Add(new SmartAirPurifier(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
        else if (entity.Type == DeviceType.RobotVacuum)
            devices.Add(new SmartRobotVacuum(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
    }
    
    return devices; // ISmartDevice listesi döndürülüyor
}
```

**Kullanım:**
```csharp
// Controller'da
var devices = _deviceService.GetAllDevices(); // IEnumerable<ISmartDevice>
foreach (var device in devices)
{
    // Her cihaz kendi GetStatus() metodunu çalıştırır
    Console.WriteLine(device.GetStatus());
}
```

**Faydası:**
- ✅ Aynı arayüz farklı davranışlar
- ✅ Yeni cihaz tipi eklemek kolay
- ✅ Loose coupling

---

### 🎯 **Abstraction (Soyutlama)**

```csharp
// Application/Interfaces/IDeviceProtocolAdapter.cs
public interface IDeviceProtocolAdapter
{
    Protocol Protocol { get; }
    
    Task<bool> PairDeviceAsync(string deviceAddress);
    Task<bool> SendCommandAsync(string deviceAddress, string command);
}

// Infrastructure/Adapters/WiFiAdapter.cs
public class WiFiAdapter : IDeviceProtocolAdapter
{
    public Protocol Protocol => Protocol.WiFi;
    
    public async Task<bool> PairDeviceAsync(string deviceAddress)
    {
        // WiFi özgü eşleşme mantığı
        await Task.Delay(2000); // Simülasyon
        return true;
    }
    
    public async Task<bool> SendCommandAsync(string deviceAddress, string command)
    {
        // WiFi özgü komut gönderme
        await Task.Delay(500);
        return true;
    }
}

// Infrastructure/Adapters/BluetoothAdapter.cs
public class BluetoothAdapter : IDeviceProtocolAdapter
{
    public Protocol Protocol => Protocol.Bluetooth;
    
    public async Task<bool> PairDeviceAsync(string deviceAddress)
    {
        // Bluetooth özgü eşleşme mantığı
        await Task.Delay(1000); // Simülasyon
        return true;
    }
    
    public async Task<bool> SendCommandAsync(string deviceAddress, string command)
    {
        // Bluetooth özgü komut gönderme
        await Task.Delay(500);
        return true;
    }
}
```

**DeviceService'te Kullanım:**
```csharp
private readonly IEnumerable<IDeviceProtocolAdapter> _adapters;

public async Task AddCustomDeviceAsync(string name, DeviceType type, string protocol, string username)
{
    // Protokol adapterini bul (soyutlama)
    var adapter = _adapters.FirstOrDefault(a => a.Protocol.ToDisplayString() == protocol);
    
    if (adapter != null)
    {
        // Hangi adapter olduğu önemli değil, hepsi aynı interface'i implement ediyor
        bool isPaired = await adapter.PairDeviceAsync("AA:BB:CC:DD:EE");
        
        if (isPaired)
        {
            // Cihazı veritabanına kaydet
            var entity = new DeviceEntity { /* ... */ };
            _context.Devices.Add(entity);
            _context.SaveChanges();
        }
    }
}
```

**Faydası:**
- ✅ İmplementasyon detayları gizli
- ✅ Yeni protokol eklemek kolay (Zigbee, Z-Wave, ...)
- ✅ Test edilebilir (Mock adapter oluşturabilirsiniz)

---

## 4. SOLID PRENSİPLERİ

### 1️⃣ **Single Responsibility Principle (SRP)**

**Tanım:** Her sınıf sadece bir sorumluluğa sahip olmalıdır.

#### ✅ **Uygulama:**

```csharp
// ❌ YANLIŞ: DeviceService hem iş mantığı hem veri erişimi yapıyor
public class DeviceService
{
    public void AddDevice(string name)
    {
        // İş mantığı
        var device = new Device { Name = name };
        
        // Veri erişimi (SRP ihlali!)
        using (var connection = new SqlConnection("..."))
        {
            // SQL kodları...
        }
    }
}

// ✅ DOĞRU: Sorumluluklar ayrıldı
public class DeviceService // İş mantığı
{
    private readonly SmartHomeDbContext _context; // Veri erişimi delegasyonu
    
    public void AddDevice(string name)
    {
        var device = new Device { Name = name };
        _context.Devices.Add(device); // DbContext'e devret
        _context.SaveChanges();
    }
}
```

**Projede Örnekler:**
- `DeviceService`: Sadece cihaz iş mantığı
- `SmartHomeDbContext`: Sadece veri erişimi
- `EventDispatcher`: Sadece event yönetimi
- `WiFiAdapter`: Sadece WiFi protokol yönetimi

---

### 2️⃣ **Open/Closed Principle (OCP)**

**Tanım:** Sınıflar genişletmeye açık, değişikliğe kapalı olmalıdır.

#### ✅ **Uygulama:**

```csharp
// ISmartDevice interface'i değişmeden yeni cihaz tipleri eklenebiliyor

// Yeni cihaz eklemek için mevcut kodları değiştirmiyoruz:
public class SmartSpeaker : ISmartDevice
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DeviceType Type => DeviceType.Speaker;
    public bool IsOn { get; set; }
    
    public void TurnOn() => IsOn = true;
    public void TurnOff() => IsOn = false;
    public string GetStatus() => IsOn ? "Hoparlör çalışıyor" : "Hoparlör kapalı";
}

// DeviceService'te sadece yeni bir "else if" ekliyoruz
public IEnumerable<ISmartDevice> GetAllDevices()
{
    // Mevcut kodlar aynı kalıyor...
    
    // Yeni cihaz tipi için sadece burası eklendi:
    else if (entity.Type == DeviceType.Speaker)
        devices.Add(new SmartSpeaker(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
    
    return devices;
}
```

**Projede Örnekler:**
- `IDeviceProtocolAdapter`: Yeni protokol eklemek için interface değişmiyor
- `IEventHandler<T>`: Yeni event handler eklemek için mevcut handler'lar değişmiyor
- `ISmartDevice`: Yeni cihaz tipi eklemek için interface değişmiyor

---

### 3️⃣ **Liskov Substitution Principle (LSP)**

**Tanım:** Alt sınıflar, üst sınıfların yerine kullanılabilmeli.

#### ✅ **Uygulama:**

```csharp
// ISmartDevice implement eden her sınıf birbirinin yerine kullanılabilir

public void ProcessDevice(ISmartDevice device)
{
    // device SmartLight, SmartThermostat veya SmartAirPurifier olabilir
    // Hepsi aynı davranışları garanti ediyor
    
    device.TurnOn(); // Hangi cihaz olursa olsun çalışır
    Console.WriteLine(device.GetStatus()); // Hangi cihaz olursa olsun çalışır
}

// Kullanım:
ProcessDevice(new SmartLight("Salon Lambası")); // ✅
ProcessDevice(new SmartThermostat("Termostat")); // ✅
ProcessDevice(new SmartAirPurifier("Hava Temizleyici")); // ✅
```

**Projede Örnekler:**
- Tüm `ISmartDevice` implementasyonları aynı davranışı garanti ediyor
- `WiFiAdapter` ve `BluetoothAdapter` birbirinin yerine kullanılabilir

---

### 4️⃣ **Interface Segregation Principle (ISP)**

**Tanım:** Sınıflar kullanmadığı metotları implement etmeye zorlanmamalı.

#### ✅ **Uygulama:**

```csharp
// ❌ YANLIŞ: Tek büyük interface
public interface IDevice
{
    void TurnOn();
    void TurnOff();
    void SetTemperature(double temp); // ❌ Sadece termostat kullansın
    void SetBrightness(int level); // ❌ Sadece ışık kullanın
    void SetSpeed(int speed); // ❌ Sadece robot süpürge kullanın
}

// ✅ DOĞRU: Küçük, özelleşmiş interface'ler
public interface ISmartDevice
{
    void TurnOn();
    void TurnOff();
    string GetStatus();
}

// Sadece ihtiyacı olanlar ekstra interface implement eder
public interface ITemperatureControllable
{
    void SetTemperature(double temp);
}

public class SmartThermostat : ISmartDevice, ITemperatureControllable
{
    public void TurnOn() { /* ... */ }
    public void TurnOff() { /* ... */ }
    public string GetStatus() { /* ... */ }
    public void SetTemperature(double temp) { /* ... */ } // Sadece termostat'ta
}

public class SmartLight : ISmartDevice // Temperature metodu yok ✅
{
    public void TurnOn() { /* ... */ }
    public void TurnOff() { /* ... */ }
    public string GetStatus() { /* ... */ }
}
```

**Projede Örnekler:**
- `ISmartDevice`: Sadece temel cihaz operasyonları
- `IDeviceProtocolAdapter`: Sadece protokol işlemleri
- `IEventHandler<T>`: Sadece spesifik event tipini handle eder

---

### 5️⃣ **Dependency Inversion Principle (DIP)**

**Tanım:** Üst seviye modüller alt seviye modüllere bağımlı olmamalı. Her ikisi de soyutlamalara bağımlı olmalı.

#### ✅ **Uygulama:**

```csharp
// ❌ YANLIŞ: Controller direkt SmartHomeDbContext'e bağımlı
public class DevicesController : ControllerBase
{
    private readonly SmartHomeDbContext _context;
    
    public DevicesController(SmartHomeDbContext context)
    {
        _context = context; // ❌ Concrete class'a bağımlı
    }
    
    [HttpGet]
    public IActionResult GetDevices()
    {
        var devices = _context.Devices.ToList(); // ❌ Direkt veri erişimi
        return Ok(devices);
    }
}

// ✅ DOĞRU: Controller interface'e bağımlı
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService; // ✅ Interface'e bağımlı
    
    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    
    [HttpGet]
    public IActionResult GetDevices()
    {
        var devices = _deviceService.GetAllDevices(); // ✅ Soyutlama üzerinden
        return Ok(devices);
    }
}

// Program.cs'te DI Container'a kayıt:
builder.Services.AddScoped<IDeviceService, DeviceService>();
// IDeviceService istendiğinde DeviceService verilir
```

**Projede Örnekler:**
```csharp
// DeviceService IDeviceProtocolAdapter'e bağımlı (concrete class'a değil)
private readonly IEnumerable<IDeviceProtocolAdapter> _adapters;

// SignalRNotificationHandler IHubContext'e bağımlı
private readonly IHubContext<DeviceNotificationHub> _hubContext;

// AutomationRuleHandler IDeviceService ve IEventDispatcher'a bağımlı
private readonly IDeviceService _deviceService;
private readonly IEventDispatcher _eventDispatcher;
```

**DI Registration (Program.cs):**
```csharp
// Services
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();

// Adapters
builder.Services.AddTransient<IDeviceProtocolAdapter, WiFiAdapter>();
builder.Services.AddTransient<IDeviceProtocolAdapter, BluetoothAdapter>();

// Event Handlers
builder.Services.AddTransient<IEventHandler<DeviceStateChangedEvent>, SignalRNotificationHandler>();
builder.Services.AddTransient<IEventHandler<DeviceStateChangedEvent>, AutomationRuleHandler>();
```

**Faydaları:**
- ✅ Unit test için mock oluşturulabilir
- ✅ İmplementasyon değiştirilebilir (SQLite → PostgreSQL)
- ✅ Loose coupling
- ✅ Flexible architecture

---

## 5. KATMANLI MİMARİ

### 📦 **Katmanlar ve Sorumlulukları**

```
┌──────────────────────────────────────────────┐
│       PRESENTATION LAYER (WPF)              │
│  - Views (XAML)                             │
│  - ViewModels (UI Logic)                    │
│  - Models (Data Transfer Objects)           │
└──────────────────────────────────────────────┘
                    ↕ HTTP/WebSocket
┌──────────────────────────────────────────────┐
│         API LAYER (Controllers)             │
│  - DevicesController                        │
│  - AuthController                           │
│  - Request/Response DTOs                    │
└──────────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────────┐
│      APPLICATION LAYER (Use Cases)          │
│  - Services (DeviceService)                 │
│  - Event Handlers (AutomationRuleHandler)   │
│  - Event Dispatcher                         │
│  - Interfaces (IDeviceService)              │
└──────────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────────┐
│       DOMAIN LAYER (Business Rules)         │
│  - Entities (SmartLight, SmartThermostat)   │
│  - Interfaces (ISmartDevice)                │
│  - Events (DeviceStateChangedEvent)         │
│  - Enums (DeviceType, Protocol)             │
│  - Value Objects                            │
└──────────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────────┐
│    INFRASTRUCTURE LAYER (Data & External)   │
│  - DbContext (SmartHomeDbContext)           │
│  - Entities (DeviceEntity, UserEntity)      │
│  - Adapters (WiFiAdapter, BluetoothAdapter) │
│  - Hubs (DeviceNotificationHub)             │
│  - Migrations                               │
└──────────────────────────────────────────────┘
                    ↓
┌──────────────────────────────────────────────┐
│             DATABASE (SQLite)               │
│  - Devices Table                            │
│  - Users Table                              │
│  - DeviceHistory Table                      │
└──────────────────────────────────────────────┘
```

### 🔄 **Katmanlar Arası Bağımlılık Kuralı:**

```
Presentation → API → Application → Domain ← Infrastructure
                                     ↑
                                     │
                            Sadece Domain'e bağımlı
```

**Kural:** Üst katmanlar alt katmanlara bağımlı. Alt katmanlar üst katmanlardan **bağımsız**.

---

## 6. DOMAIN DRIVEN DESIGN (DDD)

### 🎯 **Domain Model (İş Kuralları)**

#### **Entities (Varlıklar):**

```csharp
// Domain/Devices/SmartLight.cs
public class SmartLight : ISmartDevice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public DeviceType Type => DeviceType.Light;
    public bool IsOn { get; set; }
    
    public SmartLight(string name)
    {
        // İş kuralı: İsim boş olamaz
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Device name cannot be empty");
            
        Name = name;
    }
    
    public void TurnOn()
    {
        // İş mantığı
        IsOn = true;
    }
    
    public void TurnOff()
    {
        // İş mantığı
        IsOn = false;
    }
    
    public string GetStatus()
    {
        return IsOn ? $"{Name} açık ve ışık veriyor" : $"{Name} kapalı";
    }
}
```

#### **Value Objects:**

```csharp
// Domain/Enums/DeviceType.cs
public enum DeviceType
{
    Light = 0,
    Thermostat = 1,
    AirPurifier = 2,
    RobotVacuum = 3
}

// Domain/Enums/Protocol.cs
public enum Protocol
{
    WiFi,
    Bluetooth
}

// Domain/Extensions/ProtocolExtensions.cs
public static class ProtocolExtensions
{
    public static string ToDisplayString(this Protocol protocol)
    {
        return protocol switch
        {
            Protocol.WiFi => "WiFi",
            Protocol.Bluetooth => "Bluetooth",
            _ => protocol.ToString()
        };
    }
}
```

#### **Domain Events:**

```csharp
// Domain/Events/DomainEvent.cs (Base Class)
public abstract class DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

// Domain/Events/DeviceStateChangedEvent.cs
public class DeviceStateChangedEvent : DomainEvent
{
    public Guid DeviceId { get; }
    public string DeviceName { get; }
    public DeviceType DeviceType { get; }
    public bool IsOn { get; }
    public bool PreviousState { get; }
    public string ChangedBy { get; }
    public string Reason { get; }
    
    public DeviceStateChangedEvent(
        Guid deviceId, 
        string deviceName, 
        DeviceType deviceType,
        bool isOn, 
        bool previousState, 
        string changedBy,
        string reason = "User")
    {
        DeviceId = deviceId;
        DeviceName = deviceName;
        DeviceType = deviceType;
        IsOn = isOn;
        PreviousState = previousState;
        ChangedBy = changedBy;
        Reason = reason;
    }
}
```

#### **Domain Services:**

```csharp
// Application/Services/DeviceService.cs (Domain Service)
public class DeviceService : IDeviceService
{
    private readonly SmartHomeDbContext _context;
    private readonly IEnumerable<IDeviceProtocolAdapter> _adapters;
    private readonly IEventDispatcher _eventDispatcher;
    
    // İş mantığı: Cihaz ekleme
    public async Task AddCustomDeviceAsync(string name, DeviceType type, string protocol, string username)
    {
        // 1. Protokol adapter'ını bul
        var adapter = _adapters.FirstOrDefault(a => a.Protocol.ToDisplayString() == protocol);
        
        if (adapter != null)
        {
            // 2. Cihazı eşleştir (pair)
            bool isPaired = await adapter.PairDeviceAsync("AA:BB:CC:DD:EE");
            
            if (isPaired)
            {
                // 3. Veritabanına kaydet
                var entity = new DeviceEntity
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Type = type,
                    IsOn = false
                };
                _context.Devices.Add(entity);
                LogDeviceAction(entity.Id, name, "Eklendi", username);
                _context.SaveChanges();
                
                // 4. Event yayınla
                await _eventDispatcher.PublishAsync(new DeviceAddedEvent(
                    entity.Id, name, type, protocol, username));
            }
        }
    }
    
    // İş mantığı: Cihaz durumunu değiştirme
    public async Task ToggleDeviceAsync(Guid id, string username)
    {
        var entity = _context.Devices.FirstOrDefault(d => d.Id == id);
        if (entity != null)
        {
            bool previousState = entity.IsOn;
            entity.IsOn = !entity.IsOn;
            
            string action = entity.IsOn ? "Açıldı" : "Kapatıldı";
            LogDeviceAction(entity.Id, entity.Name, action, username);
            
            _context.SaveChanges();
            
            // Event yayınla (Automation için)
            await _eventDispatcher.PublishAsync(new DeviceStateChangedEvent(
                entity.Id, entity.Name, entity.Type, entity.IsOn, previousState, username, "User"));
        }
    }
}
```

---

## 7. EVENT-DRIVEN ARCHITECTURE (EDA)

### 📡 **Event Akışı Diyagramı**

```
Kullanıcı → WPF → API → DeviceService.ToggleDeviceAsync()
                              ↓
                    DeviceStateChangedEvent yayınlandı
                              ↓
                      EventDispatcher.PublishAsync()
                              ↓
                ┌─────────────┴─────────────┐
                ↓                           ↓
    AutomationRuleHandler        SignalRNotificationHandler
    (Otomasyon kuralı)           (SignalR broadcast)
                ↓                           ↓
    ToggleDevicesByTypeAsync()   _hubContext.Clients.All.SendAsync()
                ↓                           ↓
    Hava temizleyicileri kapat       WPF → SignalRService → MainViewModel
                ↓                           ↓
    Yeni DeviceStateChangedEvent     UI Güncellendi ✅
                ↓
    SignalRNotificationHandler
                ↓
    WPF → UI Güncellendi ✅
```

### 🔧 **Event Dispatcher İmplementasyonu:**

```csharp
// Application/Events/EventDispatcher.cs
public interface IEventDispatcher
{
    Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
}

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public EventDispatcher(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    public async Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        // Her event için yeni scope oluştur (scoped service'leri kullanabilmek için)
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
        // İlgili event için TÜM handler'ları bul
        var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>();
        
        // Her handler'ı PARALEL çalıştır
        var tasks = handlers.Select(handler => handler.HandleAsync(domainEvent));
        await Task.WhenAll(tasks);
    }
}
```

### 🎯 **Event Handler Örneği:**

```csharp
// Application/EventHandlers/AutomationRuleHandler.cs
public class AutomationRuleHandler : IEventHandler<DeviceStateChangedEvent>
{
    private readonly IDeviceService _deviceService;
    private readonly IEventDispatcher _eventDispatcher;
    
    public AutomationRuleHandler(IDeviceService deviceService, IEventDispatcher eventDispatcher)
    {
        _deviceService = deviceService;
        _eventDispatcher = eventDispatcher;
    }
    
    public async Task HandleAsync(DeviceStateChangedEvent domainEvent)
    {
        // İŞ KURALI: Robot süpürge açıldığında hava temizleyicileri kapat
        if (domainEvent.DeviceType == DeviceType.RobotVacuum && domainEvent.IsOn)
        {
            var affectedIds = await _deviceService.ToggleDevicesByTypeAsync(
                DeviceType.AirPurifier, 
                turnOn: false, 
                triggeredBy: $"Otomasyon: {domainEvent.DeviceName}");
            
            if (affectedIds.Any())
            {
                // Otomasyon tetiklendi event'i yayınla
                await _eventDispatcher.PublishAsync(new AutomationTriggeredEvent(
                    "Robot Süpürge → Hava Temizleyici Kapatma",
                    domainEvent.DeviceName,
                    affectedIds));
            }
        }
        
        // İŞ KURALI: Robot süpürge kapandığında hava temizleyicileri aç
        else if (domainEvent.DeviceType == DeviceType.RobotVacuum && !domainEvent.IsOn)
        {
            var affectedIds = await _deviceService.ToggleDevicesByTypeAsync(
                DeviceType.AirPurifier, 
                turnOn: true, 
                triggeredBy: $"Otomasyon: {domainEvent.DeviceName}");
            
            if (affectedIds.Any())
            {
                await _eventDispatcher.PublishAsync(new AutomationTriggeredEvent(
                    "Robot Süpürge → Hava Temizleyici Açma",
                    domainEvent.DeviceName,
                    affectedIds));
            }
        }
    }
}
```

### 📢 **SignalR Event Handler:**

```csharp
// Application/EventHandlers/SignalRNotificationHandler.cs
public class SignalRNotificationHandler :
    IEventHandler<DeviceStateChangedEvent>,
    IEventHandler<DeviceAddedEvent>,
    IEventHandler<DeviceRemovedEvent>,
    IEventHandler<AutomationTriggeredEvent>,
    IEventHandler<UserPresenceChangedEvent>,
    IEventHandler<EnergySavingTriggeredEvent>
{
    private readonly IHubContext<DeviceNotificationHub> _hubContext;
    
    public SignalRNotificationHandler(IHubContext<DeviceNotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task HandleAsync(DeviceStateChangedEvent domainEvent)
    {
        var message = new
        {
            Type = "DeviceStateChanged",
            DeviceId = domainEvent.DeviceId,
            DeviceName = domainEvent.DeviceName,
            IsOn = domainEvent.IsOn,
            ChangedBy = domainEvent.ChangedBy,
            Timestamp = domainEvent.OccurredAt,
            Message = $"{domainEvent.DeviceName} {(domainEvent.IsOn ? "açıldı" : "kapandı")}"
        };
        
        // TÜM bağlı client'lara broadcast
        await _hubContext.Clients.All.SendAsync("DeviceStateChanged", message);
    }
    
    // Diğer event handler'lar...
}
```

### 🔗 **Event Kayıt (Program.cs):**

```csharp
// Event Dispatcher
builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();

// DeviceStateChangedEvent için handler'lar
builder.Services.AddTransient<IEventHandler<DeviceStateChangedEvent>, SignalRNotificationHandler>();
builder.Services.AddTransient<IEventHandler<DeviceStateChangedEvent>, AutomationRuleHandler>();

// DeviceAddedEvent için handler
builder.Services.AddTransient<IEventHandler<DeviceAddedEvent>, SignalRNotificationHandler>();

// DeviceRemovedEvent için handler
builder.Services.AddTransient<IEventHandler<DeviceRemovedEvent>, SignalRNotificationHandler>();

// AutomationTriggeredEvent için handler
builder.Services.AddTransient<IEventHandler<AutomationTriggeredEvent>, SignalRNotificationHandler>();

// UserPresenceChangedEvent için handler
builder.Services.AddTransient<IEventHandler<UserPresenceChangedEvent>, SignalRNotificationHandler>();

// EnergySavingTriggeredEvent için handler
builder.Services.AddTransient<IEventHandler<EnergySavingTriggeredEvent>, SignalRNotificationHandler>();
```

---

## 8. SIGNALR - GERÇEK ZAMANLI İLETİŞİM

### 🔌 **SignalR Hub (Backend)**

```csharp
// Hubs/DeviceNotificationHub.cs
public class DeviceNotificationHub : Hub
{
    private readonly ILogger<DeviceNotificationHub> _logger;
    
    public DeviceNotificationHub(ILogger<DeviceNotificationHub> logger)
    {
        _logger = logger;
    }
    
    // Client bağlandığında
    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.Identity?.Name ?? "Guest";
        _logger.LogInformation("🔌 SignalR: {Username} bağlandı (ConnectionId: {ConnectionId})", 
            username, Context.ConnectionId);
        
        await Clients.Caller.SendAsync("Connected", new 
        { 
            Message = "SignalR bağlantısı kuruldu!",
            ConnectionId = Context.ConnectionId,
            ServerTime = DateTime.UtcNow
        });
        
        await base.OnConnectedAsync();
    }
    
    // Client bağlantıyı kopardığında
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.Identity?.Name ?? "Guest";
        
        if (exception != null)
        {
            _logger.LogWarning("❌ SignalR: {Username} bağlantıyı kesti (Hata: {Error})", 
                username, exception.Message);
        }
        else
        {
            _logger.LogInformation("❌ SignalR: {Username} bağlantıyı kesti", username);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}

// Program.cs'te kayıt
builder.Services.AddSignalR();
app.MapHub<DeviceNotificationHub>("/hubs/notifications");
```

### 📱 **SignalR Client (WPF)**

```csharp
// Services/SignalRService.cs
public class SignalRService
{
    private HubConnection? _connection;
    private readonly string _hubUrl = "https://localhost:7106/hubs/notifications";
    
    // Event'ler - ViewModel'ler bu event'leri dinleyecek
    public event EventHandler<DeviceNotificationEventArgs>? DeviceStateChanged;
    public event EventHandler<DeviceNotificationEventArgs>? DeviceAdded;
    public event EventHandler<DeviceNotificationEventArgs>? DeviceRemoved;
    public event EventHandler<string>? AutomationTriggered;
    public event EventHandler<string>? UserPresenceChanged;
    public event EventHandler<string>? EnergySavingTriggered;
    public event EventHandler<ConnectionState>? ConnectionStateChanged;
    
    public bool IsConnected => _connection?.State == HubConnectionState.Connected;
    
    public SignalRService()
    {
        InitializeConnection();
    }
    
    private void InitializeConnection()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect() // Otomatik yeniden bağlanma
            .Build();
        
        // Bağlantı durumu event'leri
        _connection.Closed += async (error) =>
        {
            ConnectionStateChanged?.Invoke(this, ConnectionState.Disconnected);
            await Task.Delay(5000);
            await StartAsync();
        };
        
        _connection.Reconnecting += (error) =>
        {
            ConnectionStateChanged?.Invoke(this, ConnectionState.Reconnecting);
            return Task.CompletedTask;
        };
        
        _connection.Reconnected += (connectionId) =>
        {
            ConnectionStateChanged?.Invoke(this, ConnectionState.Connected);
            return Task.CompletedTask;
        };
        
        // Server'dan gelen mesajları dinle
        RegisterEventHandlers();
    }
    
    private void RegisterEventHandlers()
    {
        // DeviceStateChanged event'i dinle
        _connection.On<object>("DeviceStateChanged", (message) =>
        {
            var json = System.Text.Json.JsonSerializer.Serialize(message);
            var notification = System.Text.Json.JsonSerializer.Deserialize<DeviceNotificationEventArgs>(json);
            DeviceStateChanged?.Invoke(this, notification ?? new DeviceNotificationEventArgs());
        });
        
        // DeviceAdded event'i dinle
        _connection.On<object>("DeviceAdded", (message) =>
        {
            var json = System.Text.Json.JsonSerializer.Serialize(message);
            var notification = System.Text.Json.JsonSerializer.Deserialize<DeviceNotificationEventArgs>(json);
            DeviceAdded?.Invoke(this, notification ?? new DeviceNotificationEventArgs());
        });
        
        // DeviceRemoved event'i dinle
        _connection.On<object>("DeviceRemoved", (message) =>
        {
            var json = System.Text.Json.JsonSerializer.Serialize(message);
            var notification = System.Text.Json.JsonSerializer.Deserialize<DeviceNotificationEventArgs>(json);
            DeviceRemoved?.Invoke(this, notification ?? new DeviceNotificationEventArgs());
        });
        
        // AutomationTriggered event'i dinle
        _connection.On<object>("AutomationTriggered", (message) =>
        {
            var json = System.Text.Json.JsonSerializer.Serialize(message);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var msg = doc.RootElement.GetProperty("Message").GetString() ?? "Otomasyon tetiklendi";
            AutomationTriggered?.Invoke(this, msg);
        });
        
        // UserPresenceChanged event'i dinle
        _connection.On<object>("UserPresenceChanged", (message) =>
        {
            var json = System.Text.Json.JsonSerializer.Serialize(message);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var msg = doc.RootElement.GetProperty("Message").GetString() ?? "Kullanıcı presence değişti";
            UserPresenceChanged?.Invoke(this, msg);
        });
        
        // EnergySavingTriggered event'i dinle
        _connection.On<object>("EnergySavingTriggered", (message) =>
        {
            var json = System.Text.Json.JsonSerializer.Serialize(message);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var msg = doc.RootElement.GetProperty("Message").GetString() ?? "Enerji tasarrufu aktif";
            EnergySavingTriggered?.Invoke(this, msg);
        });
    }
    
    public async Task StartAsync()
    {
        if (_connection == null) return;
        
        try
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
                ConnectionStateChanged?.Invoke(this, ConnectionState.Connected);
            }
        }
        catch (Exception ex)
        {
            ConnectionStateChanged?.Invoke(this, ConnectionState.Failed);
        }
    }
}
```

### 🖥️ **ViewModel'de SignalR Kullanımı:**

```csharp
// ViewModels/MainViewModel.cs
public partial class MainViewModel : ObservableObject
{
    private readonly SignalRService _signalRService;
    
    public MainViewModel()
    {
        _signalRService = new SignalRService();
        InitializeSignalREvents();
        _ = ConnectSignalRAsync();
    }
    
    private void InitializeSignalREvents()
    {
        // Cihaz durumu değiştiğinde
        _signalRService.DeviceStateChanged += async (sender, args) =>
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await LoadDevicesAsync(); // UI'ı güncelle
            });
        };
        
        // Yeni cihaz eklendiğinde
        _signalRService.DeviceAdded += async (sender, args) =>
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await LoadDevicesAsync();
            });
        };
        
        // Cihaz silindiğinde
        _signalRService.DeviceRemoved += async (sender, args) =>
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await LoadDevicesAsync();
            });
        };
        
        // Otomasyon tetiklendiğinde
        _signalRService.AutomationTriggered += async (sender, message) =>
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await LoadDevicesAsync();
            });
        };
        
        // Enerji tasarrufu tetiklendiğinde
        _signalRService.EnergySavingTriggered += async (sender, message) =>
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await LoadDevicesAsync();
                MessageBox.Show(message, "Enerji Tasarrufu");
            });
        };
    }
    
    private async Task ConnectSignalRAsync()
    {
        await _signalRService.StartAsync();
    }
}
```

### 🔄 **SignalR Broadcast Akışı:**

```
1. Backend: DeviceService.ToggleDeviceAsync()
   ↓
2. Event: DeviceStateChangedEvent yayınlandı
   ↓
3. Handler: SignalRNotificationHandler.HandleAsync()
   ↓
4. SignalR: _hubContext.Clients.All.SendAsync("DeviceStateChanged", message)
   ↓
5. WebSocket: Mesaj tüm bağlı client'lara gönderildi
   ↓
6. WPF: _connection.On<object>("DeviceStateChanged", ...)
   ↓
7. Event: DeviceStateChanged event tetiklendi
   ↓
8. ViewModel: LoadDevicesAsync() çağrıldı
   ↓
9. UI: ObservableCollection güncellendi → XAML otomatik güncellendi ✅
```

---

## 9. API DOKÜMANTASYONU

### 🌐 **Base URL:**
```
Development: https://localhost:7106
Production: https://api.smarthome.com (örnek)
```

### 🔐 **Authentication:**

Tüm endpoint'ler (AllowAnonymous olanlar hariç) **JWT Bearer Token** gerektirir.

**Header:**
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

### 📍 **Endpoint'ler:**

#### **1. Authentication**

##### **POST /api/auth/register**
Yeni kullanıcı kaydı.

**Request:**
```json
{
  "username": "nazlican",
  "password": "securePassword123",
  "role": "Parent"
}
```

**Response (201 Created):**
```json
{
  "message": "Kullanıcı başarıyla oluşturuldu!"
}
```

**cURL:**
```bash
curl -X POST "https://localhost:7106/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "nazlican",
    "password": "securePassword123",
    "role": "Parent"
  }'
```

---

##### **POST /api/auth/login**
Kullanıcı girişi ve JWT token alma.

**Request:**
```json
{
  "username": "nazlican",
  "password": "securePassword123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibmF6bGljYW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJQYXJlbnQiLCJleHAiOjE3NDA5MjcxNTIsImlzcyI6IlNtYXJ0SG9tZUFQSSIsImF1ZCI6IlNtYXJ0SG9tZUNsaWVudCJ9.abc123..."
}
```

**cURL:**
```bash
curl -X POST "https://localhost:7106/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "nazlican",
    "password": "securePassword123"
  }'
```

---

#### **2. Devices**

##### **GET /api/devices**
Tüm cihazları listeler.

**Authorization:** Required (Bearer Token)

**Response (200 OK):**
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Salon Lambası",
    "type": 0,
    "isOn": true
  },
  {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "name": "Yatak Odası Termostat",
    "type": 1,
    "isOn": false
  }
]
```

**cURL:**
```bash
curl -X GET "https://localhost:7106/api/devices" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

##### **POST /api/devices**
Yeni cihaz ekler.

**Authorization:** Required (Bearer Token + Parent Role)

**Request:**
```json
{
  "name": "Mutfak Lambası",
  "type": "Light",
  "protocol": "WiFi"
}
```

**Response (200 OK):**
```json
{
  "message": "Cihaz başarıyla eklendi"
}
```

**Response (401 Unauthorized):** (Child kullanıcı denerse)
```json
{
  "message": "Unauthorized"
}
```

**cURL:**
```bash
curl -X POST "https://localhost:7106/api/devices" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Mutfak Lambası",
    "type": "Light",
    "protocol": "WiFi"
  }'
```

---

##### **POST /api/devices/{id}/toggle**
Cihazı aç/kapat.

**Authorization:** Required (Bearer Token)

**Path Parameters:**
- `id` (UUID): Cihaz ID'si

**Response (200 OK):**
```json
{
  "message": "Cihaz durumu değiştirildi"
}
```

**cURL:**
```bash
curl -X POST "https://localhost:7106/api/devices/550e8400-e29b-41d4-a716-446655440000/toggle" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

##### **DELETE /api/devices/{id}**
Cihazı siler.

**Authorization:** Required (Bearer Token + Parent Role)

**Path Parameters:**
- `id` (UUID): Cihaz ID'si

**Response (200 OK):**
```json
{
  "message": "Cihaz başarıyla silindi"
}
```

**cURL:**
```bash
curl -X DELETE "https://localhost:7106/api/devices/550e8400-e29b-41d4-a716-446655440000" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

##### **POST /api/devices/presence**
Kullanıcı presence durumunu değiştirir (eve geldi/gitti).

**Authorization:** Required (Bearer Token)

**Query Parameters:**
- `isHome` (boolean): true = eve geldi, false = evden ayrıldı

**Response (200 OK):**
```json
{
  "message": "Presence durumu güncellendi"
}
```

**cURL:**
```bash
curl -X POST "https://localhost:7106/api/devices/presence?isHome=true" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

##### **GET /api/devices/history**
Cihaz geçmişini getirir.

**Authorization:** Required (Bearer Token)

**Query Parameters (Optional):**
- `deviceId` (UUID): Belirli bir cihazın geçmişi

**Response (200 OK):**
```json
[
  {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "deviceId": "550e8400-e29b-41d4-a716-446655440000",
    "deviceName": "Salon Lambası",
    "action": "Açıldı",
    "timestamp": "2026-02-23T10:30:00Z",
    "triggeredBy": "nazlican"
  },
  {
    "id": "b2c3d4e5-f678-90ab-cdef-1234567890ab",
    "deviceId": "550e8400-e29b-41d4-a716-446655440000",
    "deviceName": "Salon Lambası",
    "action": "Kapatıldı",
    "timestamp": "2026-02-23T11:00:00Z",
    "triggeredBy": "Otomasyon: Robot Süpürge"
  }
]
```

**cURL:**
```bash
# Tüm geçmiş
curl -X GET "https://localhost:7106/api/devices/history" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Belirli cihazın geçmişi
curl -X GET "https://localhost:7106/api/devices/history?deviceId=550e8400-e29b-41d4-a716-446655440000" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

##### **DELETE /api/devices/history/clear**
Tüm geçmişi temizler.

**Authorization:** Required (Bearer Token + Parent Role)

**Response (200 OK):**
```json
{
  "message": "Tüm geçmiş kayıtları başarıyla temizlendi!"
}
```

**cURL:**
```bash
curl -X DELETE "https://localhost:7106/api/devices/history/clear" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

##### **GET /api/devices/types**
Desteklenen cihaz türlerini getirir.

**Authorization:** None (AllowAnonymous)

**Response (200 OK):**
```json
[
  "Light",
  "Thermostat",
  "AirPurifier",
  "RobotVacuum"
]
```

**cURL:**
```bash
curl -X GET "https://localhost:7106/api/devices/types"
```

---

##### **GET /api/devices/protocols**
Desteklenen protokolleri getirir.

**Authorization:** None (AllowAnonymous)

**Response (200 OK):**
```json
[
  "WiFi",
  "Bluetooth"
]
```

**cURL:**
```bash
curl -X GET "https://localhost:7106/api/devices/protocols"
```

---

### 🔧 **Swagger Entegrasyonu**

#### **Swagger UI Kurulumu:**

```bash
# Package kurulumu (zaten kurulu)
dotnet add package Swashbuckle.AspNetCore
```

#### **Program.cs Güncellemesi:**

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Smart Home API", 
        Version = "v1",
        Description = "IoT tabanlı akıllı ev yönetim sistemi API",
        Contact = new OpenApiContact
        {
            Name = "Nazlican Aka",
            Email = "nazlican@example.com"
        }
    });
    
    // JWT Authentication için Swagger yapılandırması
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header kullanarak Bearer token girin. Örnek: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    
    // XML Documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Swagger Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Home API v1");
        c.RoutePrefix = string.Empty; // Swagger root'da açılsın
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<DeviceNotificationHub>("/hubs/notifications");

app.Run();
```

#### **Swagger URL:**
```
https://localhost:7106/
```

#### **Swagger JSON:**
```
https://localhost:7106/swagger/v1/swagger.json
```

---

## 10. FRONTEND-BACKEND HABERLEŞME

### 📡 **İletişim Protokolleri:**

#### **1. HTTP REST API (ApiService.cs)**

```csharp
// Services/ApiService.cs
public class ApiService
{
    private readonly HttpClient _httpClient;
    private string? _jwtToken;
    
    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7106")
        };
    }
    
    // JWT Token'ı header'a ekle
    private void SetAuthorizationHeader()
    {
        if (!string.IsNullOrEmpty(_jwtToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _jwtToken);
        }
    }
    
    // Login
    public async Task<string?> LoginAsync(string username, string password)
    {
        var loginData = new { username, password };
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginData);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            _jwtToken = result?.Token;
            SetAuthorizationHeader();
            return _jwtToken;
        }
        
        return null;
    }
    
    // Get Devices
    public async Task<List<SmartDeviceModel>> GetDevicesAsync()
    {
        SetAuthorizationHeader();
        var response = await _httpClient.GetAsync("/api/devices");
        
        if (response.IsSuccessStatusCode)
        {
            var devices = await response.Content.ReadFromJsonAsync<List<SmartDeviceModel>>();
            return devices ?? new List<SmartDeviceModel>();
        }
        
        return new List<SmartDeviceModel>();
    }
    
    // Toggle Device
    public async Task<bool> ToggleDeviceAsync(Guid deviceId)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PostAsync($"/api/devices/{deviceId}/toggle", null);
        return response.IsSuccessStatusCode;
    }
    
    // Add Device
    public async Task<bool> AddDeviceAsync(string name, string type, string protocol)
    {
        SetAuthorizationHeader();
        var deviceData = new { name, type, protocol };
        var response = await _httpClient.PostAsJsonAsync("/api/devices", deviceData);
        return response.IsSuccessStatusCode;
    }
    
    // Delete Device
    public async Task<bool> DeleteDeviceAsync(Guid deviceId)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.DeleteAsync($"/api/devices/{deviceId}");
        return response.IsSuccessStatusCode;
    }
}
```

---

#### **2. WebSocket (SignalR)**

**Backend → Frontend (Server-to-Client):**

```csharp
// Backend: SignalRNotificationHandler.cs
await _hubContext.Clients.All.SendAsync("DeviceStateChanged", message);
```

**Frontend Dinleme:**

```csharp
// Frontend: SignalRService.cs
_connection.On<object>("DeviceStateChanged", (message) =>
{
    // Mesaj geldi, işle
    DeviceStateChanged?.Invoke(this, notification);
});
```

---

### 🔄 **Tam İletişim Akışı:**

#### **Senaryo: Kullanıcı Cihaz Açıyor**

```
1. USER ACTION
   WPF → Button Click → ToggleDeviceCommand

2. HTTP REQUEST
   ApiService.ToggleDeviceAsync(deviceId)
   POST https://localhost:7106/api/devices/{id}/toggle
   Header: Authorization: Bearer {JWT_TOKEN}

3. BACKEND PROCESSING
   DevicesController.ToggleDevice()
   ↓
   DeviceService.ToggleDeviceAsync()
   ↓
   Database Update (Entity Framework)
   ↓
   EventDispatcher.PublishAsync(DeviceStateChangedEvent)

4. EVENT HANDLERS (Parallel)
   ┌─────────────────────────────────┐
   │  AutomationRuleHandler          │
   │  (Otomasyon kuralları kontrol)  │
   └─────────────────────────────────┘
   ┌─────────────────────────────────┐
   │  SignalRNotificationHandler     │
   │  (WebSocket broadcast)          │
   └─────────────────────────────────┘

5. WEBSOCKET BROADCAST
   _hubContext.Clients.All.SendAsync("DeviceStateChanged", message)
   ↓
   WebSocket → Tüm bağlı client'lara mesaj

6. FRONTEND RECEIVE
   SignalRService._connection.On("DeviceStateChanged", ...)
   ↓
   Event Trigger: DeviceStateChanged?.Invoke()

7. VIEWMODEL UPDATE
   MainViewModel.DeviceStateChanged event handler
   ↓
   Dispatcher.InvokeAsync(() => LoadDevicesAsync())

8. UI UPDATE
   ObservableCollection<SmartDeviceModel> güncellendi
   ↓
   XAML DataGrid otomatik güncellendi ✅

9. HTTP RESPONSE
   HTTP 200 OK → WPF'e döndü
   (Ama UI zaten SignalR ile güncellenmiş durumda)
```

---

## 11. CLASS VE INTERFACE DETAYLARI

### 📦 **Domain Layer Classes:**

#### **ISmartDevice Interface:**
```csharp
public interface ISmartDevice
{
    Guid Id { get; set; }
    string Name { get; set; }
    DeviceType Type { get; }
    bool IsOn { get; set; }
    void TurnOn();
    void TurnOff();
    string GetStatus();
}
```

**İmplementasyonlar:**
- `SmartLight`: Işık cihazları
- `SmartThermostat`: Termostat cihazları
- `SmartAirPurifier`: Hava temizleyici cihazları
- `SmartRobotVacuum`: Robot süpürge cihazları

---

#### **IDeviceProtocolAdapter Interface:**
```csharp
public interface IDeviceProtocolAdapter
{
    Protocol Protocol { get; }
    Task<bool> PairDeviceAsync(string deviceAddress);
    Task<bool> SendCommandAsync(string deviceAddress, string command);
}
```

**İmplementasyonlar:**
- `WiFiAdapter`: WiFi protokol yönetimi
- `BluetoothAdapter`: Bluetooth protokol yönetimi

---

### 📦 **Application Layer Classes:**

#### **IDeviceService Interface:**
```csharp
public interface IDeviceService
{
    IEnumerable<ISmartDevice> GetAllDevices();
    void TurnOnAllDevices();
    void TurnOffAllDevices();
    Task ToggleDeviceAsync(Guid id, string username);
    Task AddCustomDeviceAsync(string name, DeviceType type, string protocol, string username);
    Task RemoveDeviceAsync(Guid id, string username);
    Task TriggerPresenceAsync(bool isHome, string username);
    IEnumerable<DeviceHistoryEntity> GetDeviceHistory(Guid? deviceId = null);
    void ClearAllHistory();
    Task<List<Guid>> ToggleDevicesByTypeAsync(DeviceType deviceType, bool turnOn, string triggeredBy);
    Task TriggerEnergySavingAsync();
}
```

**İmplementasyon:**
- `DeviceService`: Tüm cihaz iş mantığı

---

#### **IEventDispatcher Interface:**
```csharp
public interface IEventDispatcher
{
    Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
}
```

**İmplementasyon:**
- `EventDispatcher`: Event yayınlama ve handler'ları çağırma

---

#### **IEventHandler<T> Interface:**
```csharp
public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent);
}
```

**İmplementasyonlar:**
- `SignalRNotificationHandler`: SignalR broadcast
- `AutomationRuleHandler`: Otomasyon kuralları

---

### 📦 **Infrastructure Layer Classes:**

#### **SmartHomeDbContext:**
```csharp
public class SmartHomeDbContext : DbContext
{
    public DbSet<DeviceEntity> Devices { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<DeviceHistoryEntity> DeviceHistory { get; set; }
    
    public SmartHomeDbContext(DbContextOptions<SmartHomeDbContext> options) 
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuration
    }
}
```

---

#### **DeviceEntity:**
```csharp
public class DeviceEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DeviceType Type { get; set; }
    public bool IsOn { get; set; }
}
```

---

#### **DeviceHistoryEntity:**
```csharp
public class DeviceHistoryEntity
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; }
    public string Action { get; set; }
    public DateTime Timestamp { get; set; }
    public string TriggeredBy { get; set; }
}
```

---

## 12. VERİTABANI VE ENTITY FRAMEWORK CORE

### 🗄️ **Database Schema:**

```sql
-- Devices Table
CREATE TABLE Devices (
    Id TEXT PRIMARY KEY,
    Name TEXT NOT NULL,
    Type INTEGER NOT NULL,
    IsOn INTEGER NOT NULL
);

-- Users Table
CREATE TABLE Users (
    Id TEXT PRIMARY KEY,
    Username TEXT NOT NULL,
    Password TEXT NOT NULL,
    Role TEXT NOT NULL
);

-- DeviceHistory Table
CREATE TABLE DeviceHistory (
    Id TEXT PRIMARY KEY,
    DeviceId TEXT NOT NULL,
    DeviceName TEXT NOT NULL,
    Action TEXT NOT NULL,
    Timestamp TEXT NOT NULL,
    TriggeredBy TEXT NOT NULL
);
```

### 🔄 **Migrations:**

```bash
# Migration oluşturma
dotnet ef migrations add InitialCreate --project SmartHome.API

# Migration uygulama
dotnet ef database update --project SmartHome.API

# Migration geri alma
dotnet ef migrations remove --project SmartHome.API
```

**Mevcut Migration'lar:**
1. `InitialCreate`: Devices tablosu
2. `AddUsersTableFixed`: Users tablosu
3. `RemoveHardcodedUsers`: Seed data kaldırıldı
4. `AddDeviceHistory`: DeviceHistory tablosu
5. `RemoveTemperatureColumn`: Temperature kolonu kaldırıldı

---

## 13. AUTHENTICATION & AUTHORIZATION

### 🔐 **JWT Implementation:**

```csharp
// Controllers/AuthController.cs
[HttpPost("login")]
[AllowAnonymous]
public IActionResult Login([FromBody] LoginRequest request)
{
    var user = _context.Users.FirstOrDefault(u => 
        u.Username == request.Username && u.Password == request.Password);
    
    if (user == null)
        return Unauthorized(new { message = "Kullanıcı adı veya şifre hatalı" });
    
    // JWT Token oluştur
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
    
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        }),
        Expires = DateTime.UtcNow.AddHours(24),
        Issuer = _configuration["Jwt:Issuer"],
        Audience = _configuration["Jwt:Audience"],
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key), 
            SecurityAlgorithms.HmacSha256Signature)
    };
    
    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);
    
    return Ok(new { token = tokenString });
}
```

### 🔒 **Authorization Attributes:**

```csharp
// Sadece Parent rolü
[Authorize(Roles = "Parent")]
[HttpPost]
public async Task<IActionResult> AddDevice([FromBody] AddDeviceDto dto) { ... }

// Herhangi bir authenticated kullanıcı
[Authorize]
[HttpGet]
public IActionResult GetAllDevices() { ... }

// Herkese açık
[AllowAnonymous]
[HttpGet("types")]
public IActionResult GetDeviceTypes() { ... }
```

---

## 14. TEST SENARYOLARI

### 🧪 **Manuel Test Senaryoları:**

#### **Senaryo 1: Kullanıcı Kaydı ve Girişi**
1. POST `/api/auth/register` ile yeni kullanıcı oluştur
2. POST `/api/auth/login` ile giriş yap
3. JWT token'ı al ve sakla
4. ✅ Başarı: Token alındı

---

#### **Senaryo 2: Cihaz Ekleme ve Listeleme**
1. POST `/api/devices` ile yeni cihaz ekle (Parent)
2. GET `/api/devices` ile tüm cihazları listele
3. ✅ Başarı: Yeni cihaz listede görünüyor

---

#### **Senaryo 3: Robot Süpürge Otomasyonu**
1. POST `/api/devices` → Robot süpürge ekle
2. POST `/api/devices` → Hava temizleyici ekle
3. POST `/api/devices/{robotId}/toggle` → Robot süpürgeyi aç
4. GET `/api/devices` → Hava temizleyici otomatik kapandı mı kontrol et
5. ✅ Başarı: Hava temizleyici `IsOn: false`

---

#### **Senaryo 4: Presence Otomasyonu**
1. Birkaç Light ve Thermostat cihazı ekle
2. POST `/api/devices/presence?isHome=true` → Eve gelindi
3. GET `/api/devices` → Tüm Light ve Thermostat'lar açıldı mı?
4. ✅ Başarı: Tüm ışıklar ve termostatlar açık

---

#### **Senaryo 5: Enerji Tasarrufu**
1. Birkaç Light cihazı ekle ve aç
2. 1 dakika bekle (Background Service)
3. GET `/api/devices` → Işıklar kapatıldı mı?
4. ✅ Başarı: Tüm ışıklar `IsOn: false`

---

#### **Senaryo 6: SignalR Gerçek Zamanlı Güncelleme**
1. WPF1: Kullanıcı A giriş yaptı
2. WPF2: Kullanıcı B giriş yaptı
3. WPF1: Cihaz açtı
4. WPF2: Ekran otomatik güncellendi mi?
5. ✅ Başarı: Her iki WPF de aynı durumu gösteriyor

---

#### **Senaryo 7: Authorization Kontrolü**
1. Child kullanıcı olarak giriş yap
2. POST `/api/devices` ile cihaz eklemeyi dene
3. ✅ Başarı: HTTP 401 Unauthorized

---

## 15. DEPLOYMENT VE ÇALIŞTIRMA

### 🚀 **Development Environment:**

#### **Gereksinimler:**
- .NET 10 SDK
- Visual Studio 2022 (veya VS Code)
- SQLite

#### **Adımlar:**

```bash
# 1. Repo'yu klonla
git clone https://github.com/NazlicanAka/SmartHomeSystem.git
cd SmartHomeSystem

# 2. Backend'i çalıştır
cd SmartHome.API
dotnet restore
dotnet run

# 3. Frontend'i çalıştır (başka terminal)
cd SmartHome.WPF
dotnet restore
dotnet run
```

---

### 📦 **Production Deployment:**

#### **Backend (API):**

```bash
# 1. Publish
dotnet publish -c Release -o ./publish

# 2. IIS / Azure App Service / Docker
# appsettings.Production.json güncellenecek:
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/app/data/smarthome.db"
  },
  "Jwt": {
    "Key": "PRODUCTION_SECRET_KEY_HERE",
    "Issuer": "SmartHomeAPI",
    "Audience": "SmartHomeClient"
  }
}

# 3. Environment Variables
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="..."
export Jwt__Key="..."
```

---

#### **Frontend (WPF):**

```bash
# 1. Publish
dotnet publish -c Release -o ./publish --self-contained true -r win-x64

# 2. İnstaller oluşturma (ClickOnce veya WiX)
# Visual Studio → Publish → ClickOnce
```

---

### 🐳 **Docker (Optional):**

```dockerfile
# Dockerfile (Backend)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SmartHome.API/SmartHome.API.csproj", "SmartHome.API/"]
RUN dotnet restore "SmartHome.API/SmartHome.API.csproj"
COPY . .
WORKDIR "/src/SmartHome.API"
RUN dotnet build "SmartHome.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartHome.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartHome.API.dll"]
```

```bash
# Docker build ve run
docker build -t smarthome-api .
docker run -d -p 8080:80 --name smarthome smarthome-api
```

---

## 📚 **EK KAYNAKLAR**

### **Proje Dosyaları:**
- `ARCHITECTURE_REVIEW.md`: Mimari detayları
- `EVENT_ARCHITECTURE.md`: Event-Driven Architecture açıklaması
- `.gitignore`: Git ignore kuralları
- `README.md`: Proje tanıtımı (oluşturulacak)

### **Swagger JSON:**
```
https://localhost:7106/swagger/v1/swagger.json
```

### **SignalR Hub URL:**
```
wss://localhost:7106/hubs/notifications
```

---

## 🎯 **SONUÇ**

Bu proje **modern yazılım geliştirme pratiklerini** kapsamlı bir şekilde uygulamaktadır:

✅ **OOP**: Encapsulation, Inheritance, Polymorphism, Abstraction  
✅ **SOLID**: Her prensibin uygulanması  
✅ **Clean Architecture**: Katmanlı mimari ve DDD  
✅ **Event-Driven**: Asenkron ve gevşek bağlı sistem  
✅ **SignalR**: Gerçek zamanlı iletişim  
✅ **RESTful API**: Standart HTTP metodları  
✅ **JWT**: Güvenli authentication  
✅ **Entity Framework Core**: ORM ve migration'lar  
✅ **Dependency Injection**: Esnek ve test edilebilir kod  
✅ **MVVM**: WPF'de temiz UI mantığı  

**Proje GitHub:** https://github.com/NazlicanAka/SmartHomeSystem

---

**Son Güncelleme:** 23 Şubat 2026  
**Yazar:** Nazlican Aka  
**Versiyon:** 1.0
