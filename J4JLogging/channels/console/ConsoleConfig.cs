using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // defines the configuration for a console channel
    public class ConsoleConfig : J4JChannelConfig<ConsoleChannel>
    {
        public ConsoleConfig()
        {
        }
    }
}