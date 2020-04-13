using JsonSubTypes;
using Newtonsoft.Json;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface ILogChannelConfiguration
    {
        LogChannel GetChannelType();
        LogEventLevel MinimumLevel { get; set; }
    }
}