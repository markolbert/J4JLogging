# Adding a New SMS Sink

You can add a new SMS sink pretty easily by using the files in the `J4JLoggingTwilio` project as a guide. Conceptually it involves creating:

- an object to hold configuration information;
- the sink itself (derived from `SmsSink`); and,
- a static extension method to simplify adding the sink to the `Serilog` `LoggerConfiguration` object
  
Here's the Twilio configuration class:

```csharp
public class TwilioConfiguration
{
    public string? AccountSID { get; set; }
    public string? AccountToken { get; set; }
    public string? FromNumber { get; set; }
    public List<string>? Recipients { get; set; }

    public bool IsValid => !string.IsNullOrEmpty( AccountSID )
                            && !string.IsNullOrEmpty( AccountToken )
                            && !string.IsNullOrEmpty( FromNumber )
                            && ( Recipients?.Any() ?? false );
}
```

The `IsValid` property lets me know if something is wrong with the configuration. If it's not true/valid an exception will be thrown by the library code when the configuration is accessed.

Here's the Twilio sink class:

```csharp
public class TwilioSink : SmsSink
{
    public TwilioSink(
        string fromNumber,
        IEnumerable<string> recipientNumbers,
        string outputTemplate
    )
        : base( outputTemplate )
    {
        FromNumber = fromNumber;
        RecipientNumbers = recipientNumbers.ToList();
    }

    public string FromNumber { get; }
    public List<string> RecipientNumbers { get; }
    public bool IsConfigured { get; internal set; }

    protected override void SendMessage( string logMessage )
    {
        if( !IsConfigured )
            throw new ArgumentException( $"{nameof(TwilioSink)} is not configured" );

        foreach( var rn in RecipientNumbers! )
        {
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
}
```

The `IsConfigured` property is defined so I can detect problems that may occur within the Twilio-provided library that the sink uses. You'll see how that's used in the next section.

The key part of this code is the `SendMessage()` override. It's what converts a text log message into an SMS message. The code shown here is one way of creating a message within Twilio. Other providers will undoubtedly use a different approach.

Finally, here's the static convenience method for adding a `TwilioSink` to `J4JLoggerConfiguration`:

```csharp
public static class TwilioExtensions
{
    public static J4JLoggerConfiguration AddTwilio(
        this J4JLoggerConfiguration loggerConfig,
        TwilioConfiguration configValues,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
        string? outputTemplate = null )
    {
        if( !configValues.IsValid )
            throw new ArgumentException( "Twilio configuration values are invalid" );

        var sink = new TwilioSink( configValues.FromNumber!, 
            configValues.Recipients!,
            outputTemplate ?? loggerConfig.GetOutputTemplate() );

        try
        {
            TwilioClient.Init( configValues.AccountSID!, configValues.AccountToken! );
            sink.IsConfigured = true;

            loggerConfig.AddSmsSink( sink, restrictedToMinimumLevel );
        }
        catch
        {
            sink.IsConfigured = false;
        }

        return loggerConfig;
    }
}
```

Two things to note here.

- I've wrapped the calls to the Twilio library's initialization routines in a try/catch block because I'm not sure what kinds of errors might occur. If the initialization fails for any reason the sink will not be marked as configured/valid/true and an exception will be thrown when an attempt is made to create an SMS message.
- The sink instance is **not** added directly to the underlying `Serilog` `LoggerConfiguration` object. Instead, it's added to `J4JLoggerConfiguration`. That's necessary so that a single instance of the enricher which indicates SMS messages may be sent is included in the `Serilog` `LoggerConfiguration` object.