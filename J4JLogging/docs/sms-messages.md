# Sending Events via SMS Messages

Any `J4JLogger` SMS sink works by including a special tag in log events. That tag triggers SMS messages to whatever SMS sinks have been added to the underlying `Serilog` logger.

You signal you want events to be sent out as SMS messages by setting the `SmsHandling` property or calling one of the provided convenience methods:

```csharp
public void SendNextEventToSms() => SmsHandling = SmsHandling.SendNextMessage;
public void SendAllEventsToSms() => SmsHandling = SmsHandling.SendUntilReset;
public void StopSendingEventsToSms() => SmsHandling = SmsHandling.DoNotSend;

public SmsHandling SmsHandling { get; set; }
```

If you set `SmsHandling` to `SmsHandling.SendNextMessage` the library automatically resets `SmsHandling` to `SmsHandling.DoNotSend` after the next log event.

If you haven't added an `SmsSink` to the underlying `Serilog` logger nothing will be transmitted. Since there is no industry-standard interface for SMS libraries each texting service requires its own channel. The **J4JLoggingTwilio** library defines one for the [Twilio service](https://www.twilio.com/).
