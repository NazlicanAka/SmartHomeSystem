using SmartHome.API.Application.Interfaces;

namespace SmartHome.API.Application.Factories
{
    /// <summary>
    /// Protokol tipine göre uygun Adapter'ı seçer (Factory Pattern)
    /// </summary>
    public interface IDeviceAdapterFactory
    {
        IDeviceProtocolAdapter GetAdapter(string protocolName);
        IEnumerable<string> GetAvailableProtocols();
    }

    public class DeviceAdapterFactory : IDeviceAdapterFactory
    {
        private readonly IEnumerable<IDeviceProtocolAdapter> _adapters;

        public DeviceAdapterFactory(IEnumerable<IDeviceProtocolAdapter> adapters)
        {
            _adapters = adapters;
        }

        public IDeviceProtocolAdapter GetAdapter(string protocolName)
        {
            var adapter = _adapters.FirstOrDefault(a => 
                a.ProtocolName.Equals(protocolName, StringComparison.OrdinalIgnoreCase));

            if (adapter == null)
            {
                throw new NotSupportedException($"Protocol '{protocolName}' is not supported");
            }

            return adapter;
        }

        public IEnumerable<string> GetAvailableProtocols()
        {
            return _adapters.Select(a => a.ProtocolName);
        }
    }
}
