# 🏠 Smart Home Management System

IoT tabanlı akıllı ev cihazlarını merkezi bir sistemden yönetmek, gerçek zamanlı izlemek ve otomasyon senaryoları kurgulamak için geliştirilmiş uçtan uca bir çözüm.

## 🚀 Öne Çıkan Özellikler

* **Güvenli Erişim:** JWT ve Rol Tabanlı Yetkilendirme (Parent/Child) altyapısı.
* **Protokol Bağımsızlığı:** Adapter Pattern ile Wi-Fi ve Bluetooth cihaz desteği.
* **Gerçek Zamanlı Takip:** SignalR entegrasyonu ile sayfa yenilemeden anlık durum güncellemeleri.
* **Akıllı Senaryolar:** Cihazlar arası etkileşim sağlayan otomatik kurgular (Örn: Robot süpürge çalışınca hava temizleyicinin kapanması).
* **Detaylı Geçmiş:** Tüm cihaz hareketlerinin kullanıcı ve sistem bazlı kayıt altına alınması.

## 🛠 Tech Stack

### **Backend**
* **.NET 10** (ASP.NET Core Web API)
* **Entity Framework Core & SQLite** (Veri Yönetimi)
* **SignalR** (Gerçek Zamanlı İletişim)

### **Frontend**
* **WPF (.NET 10)**
* **MVVM Pattern** (CommunityToolkit.Mvvm)

## 🏗 Mimari Yapı

Proje, **Clean Architecture** prensiplerine uygun olarak katmanlı bir yapıda geliştirilmiştir:
1.  **Domain:** İş kuralları ve çekirdek modeller.
2.  **Application:** Servisler ve Event-Driven işleyiciler.
3.  **Infrastructure:** Veritabanı ve dış servis adaptörleri.
4.  **Presentation:** API uç noktaları ve WPF arayüzü.

## 🛠 Kurulum ve Çalıştırma

1.  `SmartHome.API` projesini çalıştırarak veritabanının oluşmasını ve API'nin ayağa kalkmasını sağlayın.
2.  `SmartHome.WPF` projesini başlatın.
3.  Giriş ekranından bir kullanıcı oluşturun (Ebeveyn veya Çocuk) ve sistemi kullanmaya başlayın.
