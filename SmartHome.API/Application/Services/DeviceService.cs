using SmartHome.API.Application.Interfaces;
using SmartHome.API.Domain.Devices;
using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Interfaces;
using SmartHome.API.Infrastructure.Data;

namespace SmartHome.API.Application.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly SmartHomeDbContext _context;

        // Veritabanı köprümüzü (DbContext) içeri alıyoruz
        public DeviceService(SmartHomeDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ISmartDevice> GetAllDevices()
        {
            // 1. Veritabanındaki tüm kayıtları çek
            var entities = _context.Devices.ToList();
            var devices = new List<ISmartDevice>();

            // 2. Veritabanı kayıtlarını (Entity), iş kuralları nesnelerine (Domain) çevir (Mapping)
            foreach (var entity in entities)
            {
                if (entity.Type == DeviceType.Light)
                {
                    devices.Add(new SmartLight(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
                }
                else if (entity.Type == DeviceType.Thermostat)
                {
                    var thermostat = new SmartThermostat(entity.Name) { Id = entity.Id, IsOn = entity.IsOn };
                    if (entity.Temperature.HasValue) thermostat.SetTemperature(entity.Temperature.Value);
                    devices.Add(thermostat);
                }
                else if (entity.Type == DeviceType.AirPurifier) // YENİ EKLENEN
                {
                    devices.Add(new SmartAirPurifier(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
                }
                else if (entity.Type == DeviceType.RobotVacuum) // YENİ EKLENEN
                {
                    devices.Add(new SmartRobotVacuum(entity.Name) { Id = entity.Id, IsOn = entity.IsOn });
                }
            }
            return devices;
        }

        public void AddCustomDevice(string name, DeviceType type)
        {
            var entity = new DeviceEntity
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = type,
                IsOn = false
            };
            _context.Devices.Add(entity);
            _context.SaveChanges();
        }

        public void RemoveDevice(Guid id)
        {
            var entity = _context.Devices.FirstOrDefault(d => d.Id == id);
            if (entity != null)
            {
                _context.Devices.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void AddDevice(ISmartDevice device)
        {
            // Domain nesnesini veritabanı satırına (Entity) çeviriyoruz
            var entity = new DeviceEntity
            {
                Id = device.Id,
                Name = device.Name,
                Type = device.Type,
                IsOn = device.IsOn,
                Temperature = (device as SmartThermostat)?.Temperature
            };

            _context.Devices.Add(entity);
            _context.SaveChanges(); // SQL'de INSERT INTO komutunu çalıştırır
        }

        public void TurnOnAllDevices()
        {
            var entities = _context.Devices.ToList();
            foreach (var entity in entities)
            {
                entity.IsOn = true; // SQL'de UPDATE komutunu hazırlar
            }
            _context.SaveChanges(); // Veritabanına kaydeder
        }

        public void TurnOffAllDevices()
        {
            var entities = _context.Devices.ToList();
            foreach (var entity in entities)
            {
                entity.IsOn = false;
            }
            _context.SaveChanges();
        }

        public void ToggleDevice(Guid id)
        {
            var entity = _context.Devices.FirstOrDefault(d => d.Id == id);
            if (entity != null)
            {
                entity.IsOn = !entity.IsOn;
                _context.SaveChanges();
            }
        }
    }
}