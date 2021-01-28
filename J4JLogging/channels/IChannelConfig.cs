using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public interface IChannelConfig
    {
        LogEventLevel MinimumLevel { get; set; }
        string? OutputTemplate { get; set; }
        bool IsValid { get; }
        EventElements EventElements { get; set; }
        bool RequireNewline { get; set; }

        // Gets the Serilog message template in use, augmented/enriched by optional fields
        // supported by the J4JLogger system (e.g., SourceContext, which represents the 
        // source code file's path).
        string EnrichedMessageTemplate { get; }

        LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );
    }
}