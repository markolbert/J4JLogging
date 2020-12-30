using System.Text;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // defines the base configuration for a log channel
    public abstract class ChannelConfig : IChannelConfig
    {
        // the default Serilog message template to be used by the system
        public const string DefaultMessageTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}";

        protected ChannelConfig()
        {
        }

        // the minimum Serilog level the channel will log
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Verbose;
        
        public string? OutputTemplate { get; set; } = DefaultMessageTemplate;

        // flag indicating which event elements (e.g., type information, source code information)
        // will be added to the output template
        public EventElements EventElements { get; set; } = EventElements.All;

        public abstract LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );

        public virtual bool IsValid => true;

        // Gets the Serilog message template in use, augmented/enriched by optional fields
        // supported by the J4JLogger system (e.g., SourceContext, which represents the 
        // source code file's path).
        public virtual string EnrichedMessageTemplate
        {
            get
            {
                var sb = new StringBuilder( OutputTemplate );

                foreach( var element in EnumExtensions.GetUniqueFlags<EventElements>() )
                {
                    var inclElement = ( EventElements & element ) == element;

                    switch( element )
                    {
                        case EventElements.Type:
                            if( inclElement )
                                sb.Append( " {SourceContext}{MemberName}" );

                            break;

                        case EventElements.SourceCode:
                            if( inclElement )
                                sb.Append( " {SourceCodeInformation}" );

                            break;
                    }
                }

                sb.Append( "{NewLine}" );

                return sb.ToString();
            }
        }

    }
}