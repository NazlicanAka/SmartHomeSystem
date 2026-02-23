using SmartHome.API.Application.Events;
using SmartHome.API.Application.Interfaces;
using SmartHome.API.Domain.Enums;
using SmartHome.API.Domain.Events;

namespace SmartHome.API.Application.EventHandlers
{
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
            // Robot Süpürge açıldığında hava Temizleyicileri kapat
            if (domainEvent.DeviceType == DeviceType.RobotVacuum && domainEvent.IsOn)
            {
                
                var affectedIds = await _deviceService.ToggleDevicesByTypeAsync(
                    DeviceType.AirPurifier, 
                    turnOn: false, 
                    triggeredBy: $"Otomasyon: {domainEvent.DeviceName}");

                if (affectedIds.Any())
                {
                    await _eventDispatcher.PublishAsync(new AutomationTriggeredEvent(
                        "Robot Süpürge → Hava Temizleyici Kapatma",
                        domainEvent.DeviceName,
                        affectedIds));
                }
            }

            // robot süpürge kapandığında hava temizleyiciyi aç
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
}
