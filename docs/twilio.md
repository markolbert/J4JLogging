### TwilioConfig

The **Twilio channel** works by texting specially-tagged log events
to the SMS network. The tagging is done by preceding the log event you want to 
text with a call to `IJ4JLogger.IncludeSms()`. 

There is no standard API for SMS services so the details on how an SMS channel is
implemented varies from provider to provider. But the Twilio setup included in this 
package illustrates how things are done. Here's the method called to configure a
Twilio channel:
```csharp
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
```
The setup involves configuring what Serilog calls a *sub-logger* utilizing a
`TwilioSink`, which is itself a descendant of a generalized `SmsSink`:
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
All `SmsSink` does is format a log event using Serilog's built-in formatters and
send the resulting text to the abstract method `SendMessage()`.

`SendMessage()` is implemented in `TwilioSink` so that the message can be sent
using the C# API provided by Twilio:
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