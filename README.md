# J4JLogging

I'm a huge fan of [Serilog](https://serilog.net/) and use it in all of my C# work. But I wanted to be able to annotate 
the log messages with source code information because that's useful during debugging and I wanted to be able to send SMS
messages about some log events.

**J4JLogging** is my approach to doing both those things. It's a simple wrapper for Serilog which makes it easy to include caller 
information, source code information and sending text messages via Twilio for selected log events.

Apologies for the sparse documentation. Another project I'm working on is a system for creating ReadTheDocs/Sphinx style 
documentation for C# projects. But it's not ready yet :).

### Breaking Changes (v1 -> v2)

I realized recently that using a generic `IJ4JLogger<>` interface was needlessly complicated because
the only reason the type needed to be specified was to extract the type name for annotating log
entries. Since the generic interface couldn't be easily passed between types the net result was
a significant complication in using the library.

So I converted everything over to a non-generic `IJ4JLogger` interface definition. Since the library
still needs to know what type it's logging events for to annotate the event properly I implemented
an `IJ4JLoggerFactory` interface which has a single method you use like this:

```csharp
public class Simulator
{
    private readonly IJ4JLogger _logger;

    public Simulator( IJ4JLoggerFactory loggerFactory )
    {
        _logger = loggerFactory?.CreateLogger( typeof(Simulator) ) ??
                          throw new NullReferenceException( nameof(loggerFactory) );
    }
}
```

While I was doing this I decided to change the way multiple logging channels (e.g., console,
debug, file, Twilio) were handled. Rather than implemented via classes dervied from J4JLogger
I made them *channels* which you add to `J4JLogger`. Each channel takes care of configuring
the underlying Serilog logger.

This approach also let me implement a Twilio logger in a more natural way. It's built on top
of a Serilog TextWriter sink so that all of the formatting provided by Serilog for log events
comes in for free.

I apologize for not maintaining a final release of v1. But I ran into a problem I had to fix
which greatly exceeded my git command line skills, the net result of which was that I had to
blow away this repository and recreate it from a working v2.

### Configuration

I typically create instances of `IJ4JLogger` via dependency injection, including a constructor 
argument wherever I want to log stuff:

```csharp
    private readonly IJ4JLogger _logger;

    public Simulator( IJ4JLoggerFactory loggerFactory )
    {
        _logger = loggerFactory?.CreateLogger( typeof(Simulator) ) ??
                          throw new NullReferenceException( nameof(loggerFactory) );
    }
```

To create the `IJ4JLoggerFactory` I typically also use dependency injection. To make that work
you'll need to register whatever channels you're using (i.e., descendants of `ChannelConfiguration`)
and `IJ4JLoggerConfiguration`. There's a `J4JLoggerConfigurationBuilder` class to simplify
the that second registration. It's used like this:

```csharp
var configBuilder = new J4JLoggerConfigurationBuilder();

configBuilder.AddChannel( typeof(ConsoleChannel) );
configBuilder.AddChannel<DebugChannel>();

configBuilder.FromJson( jsonText );

return configBuilder.Build<SomeJ4JLoggingConfigurationType>();
```

Types that don't derive from `ChannelConfiguration` will be ignored by `AddChannel`.

Because I love the Autofac dependency injection framework I also provided some extension
methods for working with it in `AutofacExtensions`. You use them like this:

```csharp
var containerBuilder = new ContainerBuilder();

containerBuilder.AddJ4JLoggingFromFile<TConfig>(
    configFilePath,
    typeof(ConsoleConfiguration),
    typeof(DebugConfiguration),
    typeof(FileConfiguration),
    typeof(TwilioConfiguration)
);

var services = new AutofacServiceProvider(containerBuilder.Build());
```

The ultimate source of information for the configuration object in this example is a JSON configuration
file. You could, of course, define the configuration object explicitly. But I tend to put it in
a JSON file so I can tweak it at runtime without having to recompile the program.

The extension methods take care of all the necessary registrations, and tie into Net Core's
dependency injection framework, so that if you want to create an `IJ4JLoggerFactory` all
you need to do is:

```csharp
var loggerFactory = services.GetRequiredService<IJ4JLoggerFactory>();
```

The Autofac extension methods create single instances of the various
objects. That may not be strictly necessary but it seems like the right thing to do with an
approach which leads to creating a logger factory. 

### Usage

Using J4JLogger is very similar to using Serilog since it's essentially just a wrapper around an `ILogger` instance. You can use any
of Serilog's basic logging method calls, `Write()`, `Debug()`, `Error()`, `Information()`, 
`Verbose()` and `Warning()`.

Each method has overloaded variants accepting different arguments. The layout of these is identical for all the methods except Write:

```csharp
public interface IJ4JLogger<out TCalling>
{
    void Debug(
        string template,
        [ CallerMemberName ] string memberName = "",
        [ CallerFilePath ] string srcPath = "",
        [ CallerLineNumber ] int srcLine = 0
    );

    void Debug<T0>(
        string template,
        T0 propertyValue,
        [ CallerMemberName ] string memberName = "",
        [ CallerFilePath ] string srcPath = "",
        [ CallerLineNumber ] int srcLine = 0
    );

    void Debug<T0, T1>(
        string template,
        T0 propertyValue0,
        T1 propertyValue1,
        [ CallerMemberName ] string memberName = "",
        [ CallerFilePath ] string srcPath = "",
        [ CallerLineNumber ] int srcLine = 0
    );

    void Debug<T0, T1, T2>(
        string template,
        T0 propertyValue0,
        T1 propertyValue1,
        T2 propertyValue2,
        [ CallerMemberName ] string memberName = "",
        [ CallerFilePath ] string srcPath = "",
        [ CallerLineNumber ] int srcLine = 0
    );

    void Debug(
        string template,
        object[] propertyValues,
        [ CallerMemberName ] string memberName = "",
        [ CallerFilePath ] string srcPath = "",
        [ CallerLineNumber ] int srcLine = 0
    );
```

The `Write()` methods are similar, except they each start off with a Serilog `LogEventLevel` argument which specifies the log event level.

**There is one significant difference in how you call these methods from the Serilog standard, however.** If you pass anything other than a simple string (i.e., a value for the template argument) to the methods you must specify the types of the propertyValue arguments explicitly in the method call. An example:

```csharp
int someIntValue = 1;
_logger.Debug<int>("The value of that argument is {someIntValue}", someIntValue);
```

This requirement comes about because the `memberName`, `srcPath` and `srcLine` arguments are automagically set for you by the
compiler and the fact that `memberName` and `srcPath` are strings "collides" with the `template` argument.

### The TextChannel and Descendants

The `TextChannel`, which underlies the `TwilioChannel`, works by sending every logged event to 
a hidden `StringWriter`, which then extracts the most recent event's text and allows it to be further
processed. It also allows for additional configuration so that information needed by the 
post-processing event (e.g., Twilio credentials) is available.

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
the additional configuration information a derived class needs. For example, the LogTwilioChannel
class requires a configuration class defined by `ITwillioConfig`:

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

### Adjusting How Log Events Are Formatted and Which Ones Are Sent Via SMS

The `TextChannel.Clear()` method exists so that J4JLogger can allow you
to pick and choose which log events get sent via SMS. That same capability also allows
you to adjust the formatting for log events on a case-by-case basis.

It's done by calling the IJ4JLogger `Elements()` method with an enum flag indicating 
what log elements you want to include. The choices are:

```csharp
[Flags]
public enum EntryElements
{
    Assembly = 1 << 0,
    SourceCode = 1 << 1,
    ExternalSinks = 1 << 2,

    None = 0,
    All = Assembly | SourceCode | ExternalSinks
}
```

Because `Elements()` returns the `IJ4JLogger` instance you can use it inline like this:

```csharp
logger.Elements( EntryElements.All ).Information( "Fully annotated" );
```

Happy logging!
