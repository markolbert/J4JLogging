using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{
    // defines the configuration for a channel that retains the text of the last
    // even logged
    public class LastEventConfig : ChannelConfig
    {
        public string? LastLogMessage { get; private set; }

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig ) =>
            sinkConfig.LastEvent( LastEventHandler );

        private void LastEventHandler( object sender, string lastLogMessage )
        {
            LastLogMessage = lastLogMessage;
        }
    }
}