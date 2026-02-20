using Microsoft.AspNetCore.Mvc;
using SmartHome.API.Application.Interfaces;
using SmartHome.API.Domain.Devices; // Test cihazları eklemek için

namespace SmartHome.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Bu sayede adresimiz: localhost:PORT/api/devices olacak
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

        // 2. POST İsteği: Sisteme test amaçlı iki cihaz ekler
        [HttpPost("add-test-devices")]
        public IActionResult AddTestDevices()
        {
            _deviceService.AddDevice(new SmartLight("Oturma Odası Işığı"));
            _deviceService.AddDevice(new SmartThermostat("Yatak Odası Termostatı"));

            return Ok("Test cihazları sisteme başarıyla eklendi!");
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