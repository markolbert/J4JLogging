### Terminology

#### Sinks and Channels

Serilog uses the term *sink* to describe the endpoint receiving a formatted
log event. Sinks can be anything ranging from the console, a file and beyond.

Sinks, because they cover a broad array of targets, are configured in
different ways, typically by using extension methods related to the
Serilog ILogger instance or its underlying configuration. That works fine, but I wanted
a more generalized way of configuring sinks because I typically use several 
simultaneously in my projects. 

I use the term *channel* to refer to a sink that is configured to work with
IJ4JLogger. Out of the box only certain sinks have channels associated with
them:
- Console
- Debug
- File
- Twilio

But it's not hard to [create your own](channel.md) and add it to the mix.

A channel must implement `IChannelConfig`:
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

The "central repository" of channels used by a particular configuration of `IJ4JLogger`
must implement `ILogChannels`. It's very simple, just returning instances of
whatever channel configuration objects are needed:
```csharp
public interface ILogChannels : IEnumerable<IChannelConfig>
{
}
```

`LogChannels` is a class you can use as the base for your own configuration repository:
```csharp
public abstract class LogChannels : ILogChannels
{
    protected LogChannels()
    {
    }

    public abstract IEnumerator<IChannelConfig> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```

In practice you'd derive your own repository from `LogChannels` like this:
```csharp
public class ChannelConfiguration : LogChannels
{
    public ConsoleConfig Console { get; set; }
    public DebugConfig Debug { get; set; }
    public FileConfig File { get; set; }

    public override IEnumerator<IChannelConfig> GetEnumerator()
    {
        yield return Console;
        yield return Debug;
        yield return File;
    }
}
```

#### SMS Channels

`TwilioChannel` is an example of an SMS channel which sends log events as text messages.
SMS channels don't process every log event because to do so would flood the recipients.
Instead, they are intended for urgent events that demand immediate attention. 

To send a log event through the SMS channels (in addition to whatever other "normal"
channels you've defined) you call the `IncludeSms()` method on an instance of `IJ4JLogger`.
That will send the next log event, and only the next log event, to the SMS channels.
`IncludeSms()` returns the instance of `IJ4JLogger` so you can chain it:
```csharp
logger.IncludeSms().Verbose<string, string>( "{0} ({1})", "Verbose", "configPath" );
```