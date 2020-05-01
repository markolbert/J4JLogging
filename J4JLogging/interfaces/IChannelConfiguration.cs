using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface IChannelConfiguration
    {
        string Channel { get; }
        LogEventLevel MinimumLevel { get; set; }
    }
}