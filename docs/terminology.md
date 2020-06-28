### Terminology

#### Sinks and Channels

Serilog uses the term *sink* to describe the endpoint receiving a formatted
log event. Sinks can be anything ranging from the console, a file and beyond.

Sinks, because they cover a broad array of targets, are configured in
different ways, typically by using extension methods related to the
Serilog ILogger instance or its underlying configuration. Since **J4JLogger**
wraps the Serilog ILogger I needed a more generalized approach to 
configuring sinks.

I use the term channel (`LogChannel`) to refer to a sink that is configured to work with
IJ4JLogger. Out of the box only certain sinks have channels associated with
them:
- Console
- Debug
- File
- Twilio

But it's not hard to [create your own](channel.md) and add it to the mix.

#### Post-processing Channels

`TwilioChannel` is an example of a "post-processing" channel. Such channels
take the formatted text of a Serilog `LogEvent` and do something with it.
They're defined by a simple interface, `IPostProcess`:

```csharp
// defines the functionality of types which are derived from TextChannel and are hence
// capable of offering extended logging services. An example of this is the Twilio
// channel.
public interface IPostProcess
{
    // Triggers the post-processing of whatever the current contents of the log channel
    // are. Typically this would be the last LogEvent because typically PostProcess() calls
    // Clear() -- which resets the channel's state -- when it completes.
    void PostProcess();

    // clears the log channel's state
    void Clear();

    // non-generic interface for configuring the post processor
    bool Initialize( object config );
}

public interface IPostProcess<in TSms> : IPostProcess
    where TSms : class
{
    // Configures the post processor based on an instance of a configuration type
    bool Initialize( TSms config );
}
```

`PostProcess()` takes whatever formatted text has been queued up (that's
an implementation detail) and sends it somewhere, generally clearing the
queue after doing so. 

The two `Initialize()` methods allow the channel to be configured. That
doesn't have to be done when `IJ4JLogger` is first set up since the needed
information (e.g., the mobile number to text to) may not be known at that
point.