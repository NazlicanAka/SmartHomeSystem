# 📢 EVENT-DRIVEN ARCHITECTURE VE NOTIFICATION SİSTEMİ

## 🎯 EKLE OLANLAR

### **1. Domain Events (DDD Pattern)**
- ✅ `IDomainEvent` interface
- ✅ `DomainEvent` base class
- ✅ 5 Concrete Event sınıfı:
  - **DeviceStateChangedEvent**: Cihaz durumu değiştiğinde
  - **DeviceAddedEvent**: Yeni cihaz eklendiğinde
  - **DeviceRemovedEvent**: Cihaz silindiğinde
  - **AutomationTriggeredEvent**: Otomasyon tetiklendiğinde
  - **UserPresenceChangedEvent**: Kullanıcı eve geldiğinde/ayrıldığında

### **2. Event Dispatcher (Message Bus)**
- ✅ `IEventDispatcher` interface
- ✅ `EventDispatcher` implementation
- Event'leri ilgili handler'lara otomatik yönlendirir
- Birden fazla handler aynı event'i dinleyebilir

### **3. Event Handlers**
- ✅ `IEventHandler<TEvent>` generic interface
- ✅ Console logging handler'lar (geliştirme için)
- ✅ SignalR notification handler (gerçek zamanlı bildirimler)

### **4. SignalR Hub (Gerçek Zamanlı Bildirimler)**
- ✅ `DeviceNotificationHub`
- Client'lar hub'a bağlanır
- Server-side event'ler otomatik broadcast edilir
- WebSocket üzerinden çift yönlü iletişim

---

## 🏗️ MİMARİ AKIŞI

```
[User Action]
     ↓
[Controller]
     ↓
[DeviceService] -- publish --> [Event Dispatcher]
     |                               |
     |                               ↓
     |                      [Event Handlers]
     |                         /        \
     |                   [Logging]   [SignalR]
     |                                    |
     ↓                                    ↓
[Database]                    [All Connected Clients]
```

---

## 📡 SIGNAL'R HUB ENDPOINT

**Hub URL**: `https://localhost:7106/hubs/notifications`

**Metodlar (Client'tan çağrılabilir)**:
- `JoinRoom(roomName)` - Belirli bir odaya katıl

**Event'ler (Server'dan gönderilir)**:
- `DeviceStateChanged` - Cihaz durumu değişti
- `DeviceAdded` - Yeni cihaz eklendi
- `DeviceRemoved` - Cihaz silindi
- `AutomationTriggered` - Otomasyon çalıştı
- `UserPresenceChanged` - Kullanıcı presence değişti

---

## 🔥 KULLANIM ÖRNEKLERİ

### **Backend: Event Publishing**

```csharp
// DeviceService içinde
await _eventDispatcher.PublishAsync(new DeviceStateChangedEvent(
    deviceId, 
    deviceName, 
    deviceType,
    isOn, 
    previousState, 
    username,
    "User"
));
```

### **Backend: Event Handling**

```csharp
public class DeviceStateChangedLoggingHandler : IEventHandler<DeviceStateChangedEvent>
{
    public Task HandleAsync(DeviceStateChangedEvent domainEvent)
    {
        Console.WriteLine($"📝 {domainEvent.DeviceName} {domainEvent.Action}");
        return Task.CompletedTask;
    }
}
```

### **Frontend (WPF): SignalR Client Bağlantısı**

```csharp
// Önce NuGet paketi ekleyin:
// Microsoft.AspNetCore.SignalR.Client

var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7106/hubs/notifications")
    .Build();

// Event dinleme
connection.On<object>("DeviceStateChanged", (message) => 
{
    MessageBox.Show($"Cihaz durumu değişti: {message}");
});

await connection.StartAsync();
```

---

## ⚙️ DEPENDENCY INJECTION KURULUMU

```csharp
// Program.cs

// Event Dispatcher
builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();

// Event Handlers
builder.Services.AddTransient<IEventHandler<DeviceStateChangedEvent>, 
    DeviceStateChangedLoggingHandler>();
builder.Services.AddTransient<IEventHandler<DeviceStateChangedEvent>, 
    SignalRNotificationHandler>();

// SignalR
builder.Services.AddSignalR();
app.MapHub<DeviceNotificationHub>("/hubs/notifications");
```

---

## 🎨 EVENT MESSAGE FORMATI

### DeviceStateChanged
```json
{
  "Type": "DeviceStateChanged",
  "DeviceId": "guid",
  "DeviceName": "Salon Lambası",
  "IsOn": true,
  "ChangedBy": "nazlican",
  "Timestamp": "2026-02-22T10:30:00Z",
  "Message": "Salon Lambası açıldı"
}
```

### DeviceAdded
```json
{
  "Type": "DeviceAdded",
  "DeviceId": "guid",
  "DeviceName": "Yeni Lamba",
  "DeviceType": "Light",
  "AddedBy": "nazlican",
  "Timestamp": "2026-02-22T10:30:00Z",
  "Message": "Yeni cihaz eklendi: Yeni Lamba"
}
```

### AutomationTriggered
```json
{
  "Type": "AutomationTriggered",
  "AutomationName": "Robot Süpürge → Hava Temizleyici",
  "TriggerSource": "Robot Süpürge",
  "AffectedDeviceCount": 2,
  "Timestamp": "2026-02-22T10:30:00Z",
  "Message": "Otomasyon çalıştı: Robot Süpürge → Hava Temizleyici"
}
```

---

## 🚀 AVANTAJLAR

### **1. Loosely Coupled (Gevşek Bağlılık)**
- DeviceService event handler'lardan habersiz
- Yeni handler eklemek mevcut kodu değiştirmez

### **2. Scalability (Ölçeklenebilirlik)**
- Bir event için birden fazla handler çalışabilir
- Handler'lar asenkron çalışır

### **3. Real-Time Notifications**
- SignalR ile anında bildirimler
- WebSocket performansı

### **4. Auditability (İzlenebilirlik)**
- Tüm event'ler loglanır
- Sistem davranışlarını izlemek kolay

### **5. Extensibility (Genişletilebilirlik)**
- Yeni event tipi eklemek çok kolay
- Yeni handler eklemek çok kolay

---

## 📊 PATTERN KOMBİNASYONU

| Pattern | Kullanıldığı Yer |
|---------|------------------|
| **Domain Events** | DeviceStateChangedEvent, etc. |
| **Observer Pattern** | Event Handler subscription |
| **Mediator Pattern** | EventDispatcher |
| **Pub/Sub Pattern** | SignalR broadcast |
| **Strategy Pattern** | Multiple handlers per event |

---

## 🎓 İLERİ SEVİYE GELİŞTİRMELER

### **1. Persistent Event Store (EventSourcing)**
```csharp
public class EventStore
{
    public void SaveEvent(IDomainEvent domainEvent) 
    {
        // Event'leri veritabanına kaydet
        // Tüm sistem geçmişini replay edebilme
    }
}
```

### **2. Dead Letter Queue**
```csharp
// Başarısız event'leri tekrar işleme kuyruğu
public class DeadLetterQueueHandler { }
```

### **3. Event Replay**
```csharp
// Kaydedilmiş event'leri tekrar oynatma
public async Task ReplayEventsAsync(DateTime from, DateTime to) { }
```

### **4. CQRS Integration**
```csharp
// Command: Write Model
// Query: Read Model (Event'lerden türetilmiş)
```

---

## ✅ TEST SENARYOLARI

### **1. Event Publishing Testi**
```csharp
[Fact]
public async Task DeviceStateChanged_ShouldPublishEvent()
{
    // Arrange
    var dispatcher = new EventDispatcher(_serviceProvider);
    
    // Act
    await dispatcher.PublishAsync(new DeviceStateChangedEvent(...));
    
    // Assert
    // Handler'ların çağrıldığını doğrula
}
```

### **2. SignalR Integration Testi**
```csharp
[Fact]
public async Task SignalR_ShouldBroadcastToClients()
{
    // SignalR hub test
}
```

---

## 🏆 MİMARİ KALİTE (Event/Notification)

| Özellik | Seviye |
|---------|--------|
| Loosely Coupled | ⭐⭐⭐⭐⭐ |
| Scalability | ⭐⭐⭐⭐⭐ |
| Maintainability | ⭐⭐⭐⭐⭐ |
| Testability | ⭐⭐⭐⭐⭐ |
| Performance | ⭐⭐⭐⭐ |
| Real-Time | ⭐⭐⭐⭐⭐ |

---

**Hazırlayan:** AI Assistant  
**Tarih:** 2026  
**Proje:** SmartHome Event-Driven Architecture
