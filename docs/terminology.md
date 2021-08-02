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
IJ4JLogger. Out of the box only certain Serilog sinks have channels associated with
them:
- Console
- Debug
- File
- Twilio

But it's not hard to [create your own](channel.md) and add it to the mix. In fact, the
library comes with two additional sinks and their corresponding channels:
- LastEvent (makes the last Serilog event that came through the pipeline available; I often
use it in unit test)
- NetEvent (raises a standard C# event for every Serilog event; I often use it in WPF
projects to display "in process" logging messages and errors)

A channel must implement `IChannel` and be able to notify the J4JLogger it's associated
with when any of its parameters change in a way that requires the underlying Serilog
logger to be regenerated.

To simplify creating channels it's best to derive from `Channel`:
```csharp
// Base class for containing the information needed to configure an instance of FileChannel
[ChannelID("File", typeof(FileChannel))]
public class FileChannel : Channel
{
    private RollingInterval _interval = RollingInterval.Day;
    private string _folder = Environment.CurrentDirectory;
    private string _fileName = "log.txt";

    public FileChannel()
    {
        RequireNewLine = true;
    }

    public RollingInterval RollingInterval
    {
        get => _interval;
        set => SetPropertyAndNotifyLogger(ref _interval, value);
    }

    public string Folder
    {
        get => _folder;
        set => SetPropertyAndNotifyLogger(ref _folder, value);
    }

    public string FileName
    {
        get => _fileName;
        set => SetPropertyAndNotifyLogger(ref _fileName, value);
    }

    public string FileTemplatePath => Path.Combine(Folder, FileName);

    public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
    {
        return sinkConfig.File( FileTemplatePath,
            MinimumLevel,
            EnrichedMessageTemplate,
            rollingInterval: RollingInterval);
    }
}
```
The `ChannelIDAttribute` decorating the new channel is important if you want my
dependency injection framework to be able to recognize the channel. Doing so significantly
simplifies using channels. The name you assign the new channel should be unique among
all channels (the ones I've written are simply named after their sink, e.g., Console, Debug,
etc.).

#### SMS Channels

`TwilioChannel` is an example of an SMS channel which sends log events as text messages.
SMS channels don't process every log event because to do so would flood the recipients.
Instead, they are intended for urgent events that demand immediate attention. 

To send a log event through the SMS channels (in addition to whatever other "normal"
channels you've defined) you call the `OutputNextEventToSms()` method on an instance 
of `IJ4JLogger`. That will send the next log event, and only the next log event, to 
the SMS channels. 

`OutputNextEventToSms()` returns the instance of `IJ4JLogger` so you can chain it:
```csharp
logger.IncludeSms().Verbose<string, string>( "{0} ({1})", "Verbose", "configPath" );
```