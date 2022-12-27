# Twilio Sink for J4JLogger

This library provides a [Serilog](https://serilog.net/) sink which [J4JLogger](https://github.com/markolbert/J4JLogging) can use to send SMS messages via [Twilio](https://www.twilio.com/).

See the [github documentation for J4JLogger](https://github.com/markolbert/J4JLogging) for more information.

Licensed under GNU GPL-v3.0. See the [license file](../../license.md) for details.

See the [change log](changes.md) for a history of changes to the library.

## Adding a TwilioSink

Adding a `TwilioSink` to `J4JLoggerConfiguration` is straightforward. This example assumes the required configuration information is contained in a user-secrets cache since I don't want my Twilio credentials available publicly. It also does not contain any recipient phone numbers so it will not work as shown:

```csharp
var configBuilder = new ConfigurationBuilder();

var config = configBuilder
    .AddUserSecrets<LoggingTests>()
    .Build();

var twilioConfig = new TwilioConfiguration
{
    AccountSID = config.GetValue<string>( "twilio:AccountSID" ),
    AccountToken = config.GetValue<string>( "twilio:AccountToken" ),
    FromNumber = config.GetValue<string>( "twilio:FromNumber" ),
    Recipients = new List<string> { /* recipient phone numbers go here */ }
};

var loggerConfig = new J4JLoggerConfiguration( FilePathTrimmer )
    .AddTwilio( twilioConfig );
```

You can supply a `Serilog` output template string to the sink but by default it uses one that contains both calling context information (e.g., calling method name) and the placeholder required to trigger the sending of SMS messages.
