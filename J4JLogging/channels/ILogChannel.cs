using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // defines the interface that a log channel must implement. Log channel implementations must
    // also be decorated with a ChannelAttribute name/id to be valid.
    public interface ILogChannel
    {
        // the channel's name/ID, which should be unique
        string Channel { get; }

        // the minimum Serilog level the channel will log
        LogEventLevel MinimumLevel { get; set; }

        // configures the channel
        LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig, string? outputTemplate = null );
    }
}