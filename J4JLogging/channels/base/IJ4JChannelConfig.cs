using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface IJ4JChannelConfig
    {
        LogEventLevel MinimumLevel { get; set; }
        string? OutputTemplate { get; set; }
        bool IsValid { get; }
        LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );
    }
}