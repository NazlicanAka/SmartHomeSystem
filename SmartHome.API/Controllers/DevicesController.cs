using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Application.Interfaces;
using SmartHome.API.Domain.Enums;

namespace SmartHome.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Bu sayede adresimiz: localhost:PORT/api/devices olacak
    [Authorize]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        // Dependency Injection: Sistem otomatik olarak IDeviceService'i buraya verecek
        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        // 1. GET İsteği: Tüm cihazları listeler
        [HttpGet]
        public IActionResult GetAllDevices()
        {
            var devices = _deviceService.GetAllDevices();
            return Ok(devices);
        }

        [HttpPost]
        [Authorize(Roles = "Parent")] // Sadece Ebeveynler ekleyebilir
        public IActionResult AddDevice([FromBody] AddDeviceDto dto)
        {
            // Gelen string'i (örn: "Light") Enum'a çeviriyoruz
            if (Enum.TryParse<DeviceType>(dto.Type, out var deviceType))
            {
                _deviceService.AddCustomDevice(dto.Name, deviceType);
                return Ok();
            }
            return BadRequest("Geçersiz cihaz türü.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Parent")] // Sadece Ebeveynler silebilir
        public IActionResult DeleteDevice(Guid id)
        {
            _deviceService.RemoveDevice(id);
            return Ok();
        }

        // Kullanıcıdan gelecek verinin şablonu
        public class AddDeviceDto
        {
            public string Name { get; set; }
            public string Type { get; set; }
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
        public IActionResult ToggleDevice(Guid id)
        {
            _deviceService.ToggleDevice(id);
            return Ok();
        }
    }
}