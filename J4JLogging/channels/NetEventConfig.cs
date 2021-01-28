using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Display;

namespace J4JSoftware.Logging
{
    // defines the configuration for a channel that retains the text of the last
    // even logged
    public class NetEventConfig : ChannelConfig
    {
        public const string DefaultNetEventConfigOutputTemplate = "[{Level:u3}] {Message}";

        public NetEventConfig()
        {
            OutputTemplate = DefaultNetEventConfigOutputTemplate;
            EventElements = EventElements.None;
            RequireNewline = false;
        }

        public event EventHandler<NetEventArgs>? LogEvent;

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.NetEvent( new MessageTemplateTextFormatter( EnrichedMessageTemplate ), this );
        }

        internal void OnLogEvent( NetEventArgs args ) => LogEvent?.Invoke( this, args );
    }
}