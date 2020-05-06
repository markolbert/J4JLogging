# J4JLogging

I'm a huge fan of [Serilog](https://serilog.net/) and use it in all of my C# work. But I 
wanted to be able to annotate the log messages with source code information because that's 
useful during debugging and I wanted to be able to send SMS messages about some log events.

**J4JLogging** is my approach to doing both those things. It's a simple wrapper for Serilog 
which makes it easy to include caller information, source code information and sending text 
messages via Twilio for selected log events.

Apologies for the sparse documentation. Another project I'm working on is a system for 
creating ReadTheDocs/Sphinx style documentation for C# projects. But it's not ready yet :).

### Usage

I typically create instances of `IJ4JLogger` via dependency injection, including a constructor 
argument wherever I want to log stuff:

```csharp
    private readonly IJ4JLogger _logger;

    public Simulator( IJ4JLogger logger )
    {
        _logger = logger ?? throw new NullReferenceException( nameof(logger) );

        _logger.SetLoggedType( this.GetType() );_
    }
```

`SetLoggedType()` isn't required. But it's necessary if you want to include information
about the type of object generating logging information.

Using J4JLogger is very similar to using Serilog since it's essentially just a wrapper 
around an `ILogger` instance. You can use any of Serilog's basic logging method calls, 
`Write()`, `Debug()`, `Error()`, `Information()`, `Verbose()` and `Warning()`.

Each method has overloaded variants accepting different arguments. The layout of these 
is identical for all the methods except Write:

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

The `Write()` methods are similar, except they each start off with a Serilog `LogEventLevel` 
argument which specifies the log event level.

**There is one significant difference in how you call these methods from the Serilog 
standard, however.** If you pass anything other than a simple string (i.e., a value 
for the template argument) to the methods you must specify the types of the propertyValue 
arguments explicitly in the method call. An example:

```csharp
int someIntValue = 1;
_logger.Debug<int>("The value of that argument is {someIntValue}", someIntValue);
```

This requirement comes about because the `memberName`, `srcPath` and `srcLine` arguments 
are automagically set for you by the compiler and the fact that `memberName` and `srcPath` 
are strings "collides" with the `template` argument.

### IJ4JLoggingConfiguration Builders

To create instances of `IJ4JLogger` I typically use dependency injection. To make that work
you'll need to register whatever channels you're using (i.e., classes implementing
`ILogChannel`) and the class implementing `IJ4JLoggerConfiguration` as well as the
class implementing `IJ4JLogger`.

Registering a class for `IJ4JLogger` should be pretty straightforward in any dependency
injection framework. Registering a class implementing `IJ4JLoggerConfiguration` is more
involved so I've included some builders for creating instances you can register.

#### Derived Configuration Builder

The first builder creates an instance of `J4JLoggerConfiguration` from a JSON file whose
structure matches a class implementing `IJ4JLoggerConfiguration`. That could look like this:
```
{
  "DefaultElements" :  "SourceCode", 
  "SourceRootPath": "C:/Programming/J4JLogging/",
  "Channels": [
    {
      "Channel": "Console",
      "MinimumLevel": "Verbose"
    }
  ]
}
```
which could be directly deserialized to an instance of `J4JLoggerConfiguration`. Or the
JSON file might look like this:
```
{
  "SomeOtherProperty": true,
  "SomeOtherArray": [
    "a",
    "b",
    "c"
  ],
  "SomeOtherObject" : {
    "Property1": 15,
    "Property2": "abc" 
  },
  "SourceRootPath": "C:/Programming/J4JLogging/",
  "Channels": [
    {
      "Channel": "Console",
      "MinimumLevel": "Verbose"
    }
  ]
}
```
which would be deserialized to a class derived from `J4JLoggerConfiguration`.

In either case the builder is used like this:
```
var configBuilder = new J4JLoggerConfigurationJsonBuilder();

foreach( var kvp in channels )
{
    configBuilder.AddChannel( kvp.Value );
}

configBuilder.FromFile( configFilePath );

return configBuilder.Build<TConfig>();
```
where `TConfig` is the class implementing `IJ4JLoggerConfiguration`. The return value is
an instance of `TConfig` suitable for registering in a dependency injection framework.

#### Embedded Configuration Builder

That first type of configuration builder won't work if the logging information is contained 
("embedded") in a property of a larger configuration class. Here's an example of such a 
JSON file:
```
{
  "SomeOtherProperty": true,
  "SomeOtherArray": [
    "a",
    "b",
    "c"
  ],
  "Logger": {
    "SourceRootPath": "C:/Programming/J4JLogging/",
    "EventElements" :  "All", 
    "Channels": [
      {
        "Channel": "Console",
        "MinimumLevel": "Verbose"
      }
    ]
  }
}
```

Because this type of configuration structure is common -- and is often used with the 
`Microsoft.Extensions.Configuration` framework -- I've included a second configuration 
builder which works off of an `IConfigurationRoot` object containing configuration
information. You use it like this:

```
// services is an instance of IServiceProvider
var configRoot = services.GetRequiredService<IConfigurationRoot>();

var loggerBuilder = new J4JLoggerConfigurationRootBuilder();

foreach( var channelType in channelTypes )
{
    loggerBuilder.AddChannel( channelType );
}

// loggerSection is the name of the property holding the logging configuration
// information
return loggerBuilder.Build<TConfig>( configRoot, loggerSection );
```
The first line gets an instance of `IConfigurationRoot` from your dependency injection
framework. You'll have to have registered it with the DI framework, of course.

The last line returns an instance of the class implementing `IJ4JLoggingConfiguration` 
which can registered in your dependency injection framework. Typically this would be 
`J4JLoggingConfiguration` itself.

### Autofac Support

Because I love the [Autofac dependency injection framework](https://autofac.org/) I've 
provided some extension methods to simplify setting up J4JLogging with `Autofac`. 

Here's an example when the logging configuration info is in a JSON file defining a
class implementing `IJ4JLoggerConfiguration`:

```csharp
var containerBuilder = new ContainerBuilder();

containerBuilder.AddJ4JLogging<TConfig>(
    configFilePath,
    typeof(ConsoleChannel),
    typeof(DebugChannel),
    typeof(FileChannel),
    typeof(TwilioChannel)
);
```
`TConfig` is a class implementing `IJ4JLoggerConfiguration` and `configFilePath` is
a string defining the location of the JSON configuration file. You can add as many
channels as you wish.

When the logging configuration info is in a property of a JSON file which will be
used with the `Microsoft.Extensions.Configuration` API you'd use a pattern like this:
```
var builder = new ContainerBuilder();

var configRoot = new ConfigurationBuilder()
    .AddJsonFile( configFilePath )
    .Build();

builder.Register(c => configRoot.Get<Configuration>())
    .AsSelf()
    .SingleInstance();

builder.AddJ4JLogging<J4JLoggerConfiguration>(
    configRoot,
    "Logger", 
    typeof(ConsoleChannel), typeof(FileChannel) );
```
You don't need to register the overall configuration object (`Configuration` in this
example) unless you want to access it via dependency injection. You also can add as many
channels as you wish.

The `Autofac` extension methods create single instances of `IJ4JLoggerConfiguration` and
`ILogger` (the underlying `Serilog` logger). That's the pattern I typically use but your
use case may be different.

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
### Specifying Event Elements
The `EventElements` property of `IJ4JLoggerConfiguration` lets you define whether or not
to include source code detail and type/caller detail in the log events. It's a flag `Enum`:
```
[Flags]
public enum EventElements
{
    Type = 1 << 0,
    SourceCode = 1 << 1,

    None = 0,
    All = Type | SourceCode
}
```
When specifying multiple flags in a JSON config file you separate the elements with 
commas:
```
"Logger": {
    "SourceRootPath": "C:/Programming/J4JLogging/",
    "EventElements" :  "SourceCode, Type", 
    ...
```



Happy logging!
