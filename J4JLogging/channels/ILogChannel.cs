using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface ILogChannel
    {
        string Channel { get; }
        LogEventLevel MinimumLevel { get; set; }
        LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig, string outputTemplate = null );
    }
}