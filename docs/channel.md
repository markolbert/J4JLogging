### Adding a Channel

Channels are a concept I added to the Serilog environment because I wanted to be able 
to configure multiple Serilog sinks in a generalized way. Adding a channel is easy:
you define a configuration class for the channel which implements `IChannelConfig`:
```csharp
public interface IChannelConfig
{
    LogEventLevel MinimumLevel { get; set; }
    string? OutputTemplate { get; set; }
    bool IsValid { get; }
        
    // Gets the Serilog message template in use, augmented/enriched by optional fields
    // supported by the J4JLogger system (e.g., SourceContext, which represents the 
    // source code file's path).
    string EnrichedMessageTemplate { get; }

    LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );
}
```
To make this even easier there's a base class, `ChannelConfig`, from which you
derive your own channel configuration class:
```csharp
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
```
The only method you must implement/override is `Configure()`. You can also add 
whatever additional configuration properties are needed for a particular channel. 
A good example of this is `FileConfig`, which holds the information for configuring 
a rolling log file sink and "knows" how to configure that particular sink.
