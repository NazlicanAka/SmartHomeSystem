using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Application.Interfaces;
using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Extensions;

namespace SmartHome.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Bu sayede adresimiz: localhost:PORT/api/devices olacak
    [Authorize]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly IEnumerable<IDeviceProtocolAdapter> _adapters;

        // Dependency Injection: Sistem otomatik olarak IDeviceService'i buraya verecek
        public DevicesController(IDeviceService deviceService, IEnumerable<IDeviceProtocolAdapter> adapters)
        {
            _deviceService = deviceService;
            _adapters = adapters;
        }

        // 1. GET İsteği: Tüm cihazları listeler
        [HttpGet]
        public IActionResult GetAllDevices()
        {
            var devices = _deviceService.GetAllDevices();
            return Ok(devices);
        }

        [HttpPost]
        [Authorize(Roles = "Parent")]
        public async Task<IActionResult> AddDevice([FromBody] AddDeviceDto dto)
        {
            if (Enum.TryParse<DeviceType>(dto.Type, out var deviceType))
            {
                // JWT'den kullanıcı adını al
                var username = User.Identity?.Name ?? "Bilinmeyen";

                await _deviceService.AddCustomDeviceAsync(dto.Name, deviceType, dto.Protocol, username);
                return Ok();
            }
            return BadRequest("Geçersiz cihaz türü.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Parent")] // Sadece Ebeveynler silebilir
        public async Task<IActionResult> DeleteDevice(Guid id)
        {
            // JWT'den kullanıcı adını al
            var username = User.Identity?.Name ?? "Bilinmeyen";

            await _deviceService.RemoveDeviceAsync(id, username);
            return Ok();
        }

        // Kullanıcıdan gelecek verinin şablonu
        public class AddDeviceDto
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Protocol { get; set; } // Wi-Fi veya Bluetooth
        }

        // 3. POST İsteği: "Eve Geldim" senaryosu - Tüm cihazları açar
        [HttpPost("turn-on-all")]
        public IActionResult TurnOnAll()
        {
            _deviceService.TurnOnAllDevices();
            return Ok("Eve hoş geldin! Tüm cihazlar açıldı.");
        }

        // 4. POST İsteği: Belirli bir cihazı açıp kapatır
        [HttpPost("{id}/toggle")]
        public async Task<IActionResult> ToggleDevice(Guid id)
        {
            // JWT'den kullanıcı adını al
            var username = User.Identity?.Name ?? "Bilinmeyen";

            await _deviceService.ToggleDeviceAsync(id, username);
            return Ok();
        }

        [HttpPost("presence")]
        public async Task<IActionResult> TriggerPresence([FromQuery] bool isHome)
        {
            // JWT'den kullanıcı adını al
            var username = User.Identity?.Name ?? "Bilinmeyen";

            await _deviceService.TriggerPresenceAsync(isHome, username);
            return Ok();
        }

        // 📊 GET İsteği: Cihaz geçmişini getir
        // Kullanım: /api/devices/history (tüm cihazlar için)
        // Kullanım: /api/devices/history?deviceId=xxx (belirli cihaz için)
        [HttpGet("history")]
        public IActionResult GetDeviceHistory([FromQuery] Guid? deviceId = null)
        {
            var history = _deviceService.GetDeviceHistory(deviceId);
            return Ok(history);
        }

        // 🗑️ DELETE İsteği: Tüm geçmişi temizle
        // Kullanım: /api/devices/history/clear
        [HttpDelete("history/clear")]
        [Authorize(Roles = "Parent")] // Sadece Ebeveynler temizleyebilir
        public IActionResult ClearHistory()
        {
            _deviceService.ClearAllHistory();
            return Ok(new { Message = "Tüm geçmiş kayıtları başarıyla temizlendi!" });
        }

        // 📋 GET İsteği: Desteklenen cihaz türlerini getir
        // Kullanım: /api/devices/types
        [HttpGet("types")]
        [AllowAnonymous] // Giriş yapmadan da erişilebilir (kayıt ekranı için)
        public IActionResult GetDeviceTypes()
        {
            var deviceTypes = Enum.GetNames(typeof(DeviceType));
            return Ok(deviceTypes);
        }

        // 📋 GET İsteği: Desteklenen protokolleri getir (DI'dan dinamik olarak)
        // Kullanım: /api/devices/protocols
        [HttpGet("protocols")]
        [AllowAnonymous]
        public IActionResult GetProtocols()
        {
            // ✅ Loose Coupling: DI container'daki kayıtlı adapter'lardan protokolleri al
            var protocols = _adapters
                .Select(a => a.Protocol.ToDisplayString())
                .Distinct()
                .ToList();

            return Ok(protocols);
        }
    }
}