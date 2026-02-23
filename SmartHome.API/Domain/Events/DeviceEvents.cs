using SmartHome.API.Domain.Enums;

namespace SmartHome.API.Domain.Events
{

    // DomainEvent abstract classından inherit olan classlardır. Asıl olaylardır.
    public class DeviceStateChangedEvent : DomainEvent
    {
        public Guid DeviceId { get; }
        public string DeviceName { get; }
        public DeviceType DeviceType { get; }
        public bool IsOn { get; }
        public bool PreviousState { get; }
        public string ChangedBy { get; }
        public string Reason { get; } // "User", "Automation", "Presence"

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

    public class DeviceAddedEvent : DomainEvent
    {
        public Guid DeviceId { get; }
        public string DeviceName { get; }
        public DeviceType DeviceType { get; }
        public string Protocol { get; }
        public string AddedBy { get; }

        public DeviceAddedEvent(Guid deviceId, string deviceName, DeviceType deviceType, string protocol, string addedBy)
        {
            DeviceId = deviceId;
            DeviceName = deviceName;
            DeviceType = deviceType;
            Protocol = protocol;
            AddedBy = addedBy;
        }
    }

    public class DeviceRemovedEvent : DomainEvent
    {
        public Guid DeviceId { get; }
        public string DeviceName { get; }
        public string RemovedBy { get; }

        public DeviceRemovedEvent(Guid deviceId, string deviceName, string removedBy)
        {
            DeviceId = deviceId;
            DeviceName = deviceName;
            RemovedBy = removedBy;
        }
    }

    public class DeviceDisconnectedEvent : DomainEvent
    {
        public Guid DeviceId { get; }
        public string DeviceName { get; }
        public string Reason { get; }

        public DeviceDisconnectedEvent(Guid deviceId, string deviceName, string reason)
        {
            DeviceId = deviceId;
            DeviceName = deviceName;
            Reason = reason;
        }
    }

    public class AutomationTriggeredEvent : DomainEvent
    {
        public string AutomationName { get; }
        public string TriggerSource { get; }
        public List<Guid> AffectedDevices { get; }

        public AutomationTriggeredEvent(string automationName, string triggerSource, List<Guid> affectedDevices)
        {
            AutomationName = automationName;
            TriggerSource = triggerSource;
            AffectedDevices = affectedDevices;
        }
    }

    public class UserPresenceChangedEvent : DomainEvent
    {
        public string Username { get; }
        public bool IsHome { get; }
        public int AffectedDeviceCount { get; }

        public UserPresenceChangedEvent(string username, bool isHome, int affectedDeviceCount)
        {
            Username = username;
            IsHome = isHome;
            AffectedDeviceCount = affectedDeviceCount;
        }
    }

    public class EnergySavingTriggeredEvent : DomainEvent
    {
        public int DevicesAffected { get; }
        public List<Guid> AffectedDeviceIds { get; }

        public EnergySavingTriggeredEvent(int devicesAffected, List<Guid> affectedDeviceIds)
        {
            DevicesAffected = devicesAffected;
            AffectedDeviceIds = affectedDeviceIds;
        }
    }
}
