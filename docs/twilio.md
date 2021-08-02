### TwilioConfig

The **Twilio channel** works by texting specially-tagged log events
to the SMS network. The tagging is done by preceding the log event you want to 
text with a call to `IJ4JLogger.OutputNextEventToSms()`. 

Of course, if you haven't included a supported SMS channel to the logger nothing 
will be transmitted. Since there is no industry-standard interface for SMS libraries
each texting service requires its own channel. The **J4JLoggingTwilio** library
defines one for the [Twilio service](https://www.twilio.com/). You can use it as
a guide to create your own SMS channel.

The key is to create a Serilog *sink* and a custom **J4JLogger** channel. You can use
the provided `SmsSink` as a basis for your own:
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
    public ITextFormatter? TextFormatter { get; internal set; }

    public void Emit( LogEvent logEvent )
    {
        if( TextFormatter == null )
            return;

        _sb.Clear();
        TextFormatter.Format(logEvent, _stringWriter);
        _stringWriter.Flush();
            
        SendMessage( _sb.ToString() );
    }

    protected abstract void SendMessage( string logMessage );
}
```
The layout mirrors Twilio's API for sending a simple SMS message but it should be generally
applicable. The key part lies in the `Emit()` method, which is called by the Serilog API to
format messages destined for your SMS sink.

To work specifically with Twilio `SmsSink` needs to be extended. That's done in `TwilioSink`:
```csharp
public class TwilioSink : SmsSink
{
    protected override void SendMessage( string logMessage )
    {
        foreach( var rn in RecipientNumbers )
            try
            {
                MessageResource.Create( body: logMessage, to: rn, @from: FromNumber );
            }
            catch( Exception e )
            {
                throw new InvalidOperationException(
                    $"Could not create Twilio message. Exception message was '{e.Message}'" );
            }
    }
}
```
`MessageResource.Create()` is part of the Twilio API.

You also have to create a `J4JLogger` channel for your SMS system. The provided one for Twilio
looks like this:
```csharp
[ChannelID("Twilio", typeof(TwilioChannel))]
public class TwilioChannel : Channel
{
    public string? AccountSID { get; set; }
    public string? AccountToken { get; set; }
    public string? FromNumber { get; set; }
    public List<string>? Recipients { get; set; }

    public bool IsValid
    {
        get
        {
            if( string.IsNullOrEmpty( AccountSID ) ) return false;
            if( string.IsNullOrEmpty( AccountToken ) ) return false;
            if( string.IsNullOrEmpty( FromNumber ) ) return false;

            return Recipients?.Count != 0;
        }
    }

    public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
    {
        if( !IsValid )
            throw new ArgumentException(
                "Could not configure the Twilio channel because one or more required parameters required by Twilio were not defined locally" );

        TwilioClient.Init( AccountSID, AccountToken );

        return sinkConfig.Logger( lc => lc.Filter
            .ByIncludingOnly( "SendToSms" )
            .WriteTo
            .Sms<TwilioSink>(
                new MessageTemplateTextFormatter( EnrichedMessageTemplate ),
                FromNumber!,
                Recipients! ) );
    }
}
```
The call to `Configure()` takes advantage of Serilog's *sublogger* functionality to add the 
Twilio sink to the Serilog pipeline. It does that by calling the `Sms()` extension method defined
in the **J4JLogger** library.
```