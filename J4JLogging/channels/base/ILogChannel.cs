using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface ILogChannel
    {
        LogEventLevel MinimumLevel { get; }
        string? OutputTemplate { get; }
        LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );
    }
}