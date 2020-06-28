### The TextChannel and Descendants

The `TextChannel`, which underlies the `TwilioChannel`, works by sending every logged 
event to a hidden `StringWriter`, which then extracts the most recent event's text and 
allows it to be further processed. It also allows for additional configuration so that 
information needed by the post-processing event (e.g., Twilio credentials) is available.

Here are the `TextChannel` methods which do this:

```csharp
public virtual bool Initialize( TSms config ) => true;

public void PostProcess()
{
    ProcessLogMessage(_writer.ToString());
    ClearLogEventText();
}

public void Clear()
{
    _writer.GetStringBuilder().Clear();
}

protected virtual bool ProcessLogMessage( string mesg ) => true;
```

`TextChannel` is a generic class, with the generic class parameter being the type holding
the additional configuration information a derived class needs. For example, the 
TwilioChannel class requires a configuration class defined by `ITwillioConfig`:

```csharp
public interface ITwilioConfig
{
    string AccountSID { get; }
    string AccountToken { get; }
    string FromNumber { get; }
    List<string> Recipients { get; }

    bool IsValid { get; }

    PhoneNumber GetFromNumber();
    List<PhoneNumber> GetRecipients();
}
```

You can find a basic implementation of this interface in the class `TwilioConfig`.

The `TwilioChannel` channel does its magic by overriding the `TextChannel`'s
`ProcessLogMessage()`:

```csharp
protected override bool ProcessLogMessage( string mesg )
{
    if( _config == null )
        return false;

    var fromNumber = _config.GetFromNumber();

    _config.GetRecipients()
        .ForEach( r => MessageResource.Create(
            body : mesg,
            to : r,
            from : fromNumber )
        );

    return true;
}
```
