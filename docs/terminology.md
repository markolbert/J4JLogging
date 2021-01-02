### Terminology

#### Sinks and Channels

Serilog uses the term *sink* to describe the endpoint receiving a formatted
log event. Sinks can be the console, a file or something else.

Sinks, because they cover a broad array of targets, are configured in
different ways, typically by using extension methods related to the
Serilog ILogger instance or its underlying configuration. That works fine, but I 
wanted a more generalized way of configuring sinks because I typically use several 
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

#### SMS Channels

`TwilioConfig` is an example of an SMS channel which sends log events as text messages.
SMS channels don't process every log event because to do so would flood the recipients.
Instead, they are intended for urgent events that demand immediate attention. 

To send a log event through the SMS channels (in addition to whatever other "normal"
channels you've defined) you call the `IncludeSms()` method on an instance 
of `IJ4JLogger`. That will send the next log event, and only the next log event, to 
the SMS channels. 

`IncludeSms()` returns the instance of `IJ4JLogger` so you can chain it:
```csharp
logger.IncludeSms().Verbose<string, string>( "{0} ({1})", "Verbose", "configPath" );
```