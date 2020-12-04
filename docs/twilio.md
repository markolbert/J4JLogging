### The SmsSink and TwilioConfig

The `SmsSink` works by texting specially-tagged (via a call to `IJ4JLogger.IncludeSms()`) log events
to the SMS network. Knowing how to do that differs depending upon the library you're using to 
access the SMS network. Putting the pieces together involves deriving a library-specific sink
from `SmsSink` and defining the appropriate configuration class for it which implements
`IChannelConfig`.

The Twilio setup included in this package illustrates how this is done. Here's `TwilioSink`,
which handles the actual message transmission:
```csharp
public class TwilioSink : SmsSink
{
    protected override void SendMessage( string logMessage )
    {
        foreach( var rn in RecipientNumbers )
        {
            try
            {
                MessageResource.Create( body : logMessage, to : rn, @from : FromNumber );
            }
            catch( Exception e )
            {
                throw new InvalidOperationException(
                    $"Could not create Twilio message. Exception message was '{e.Message}'" );
            }
        }
    }
}
```
The `RecipientNumbers` and `FromNumber` properties are defined in `SmsSink`:
```csharp
public abstract class SmsSink : ILogEventSink
{
    private readonly StringWriter _stringWriter;
    private readonly StringBuilder _sb;

    protected SmsSink()
    {
        _sb = new StringBuilder();
        _stringWriter = new StringWriter( _sb );
    }

    public string FromNumber { get; internal set; } = string.Empty;
    public List<string> RecipientNumbers { get; internal set; } = new List<string>();
    public ITextFormatter TextFormatter { get; internal set; }

    public void Emit( LogEvent logEvent )
    {
        _sb.Clear();
        TextFormatter.Format(logEvent, _stringWriter);
        _stringWriter.Flush();
            
        SendMessage( _sb.ToString() );
    }

    protected abstract void SendMessage( string logMessage );
}
```
The code involving the `StringWriter` and `StringBuilder` simply stores
each log event -- after formatting it -- and then sends the stored
string to the `SendMessage()` method. The reason for the somewhat convoluted
approach is so we can take advantage of Serilog's built-in formatting
capabilities. They depend on being able to write formatted output to
a stream, in this case an instance of `StringWriter`.

The channel configuration side of things is handled by `TwilioConfig`:
```csharp
// Base class for containing the information needed to configure an instance of TwilioChannel
public class TwilioConfig : ChannelConfig
{
    public string AccountSID { get; set; }
    public string AccountToken { get; set; }
    public string FromNumber { get; set; }
    public List<string> Recipients { get; set; }

    public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
    {
        TwilioClient.Init(AccountSID, AccountToken);

        return sinkConfig.Logger( lc => lc.Filter
            .ByIncludingOnly( "SendToSms" )
            .WriteTo
            .Sms<TwilioSink>( 
                new MessageTemplateTextFormatter( EnrichedMessageTemplate ), 
                FromNumber,
                Recipients ) );
    }

    public override bool IsValid
    {
        get
        {
            if( string.IsNullOrEmpty( AccountSID ) ) return false;
            if( string.IsNullOrEmpty( AccountToken ) ) return false;
            if( string.IsNullOrEmpty( FromNumber ) ) return false;

            return Recipients.Count != 0;
        }
    }
}
```
Notice that the `SmsSink` is set up as a "sub-logger". That's so we can take advantage
of Serilog's built-in log event filtering and ensure only those events marked with
a *SendToSms* property with the value *true* get handled by the SMS sink. If we didn't do
this our SMS recipients would be flooded with log messages.
