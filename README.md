# J4JLogging

I'm a huge fan of [Serilog](https://serilog.net/) and use it in all of my C# work. But I wanted to be able to annotate 
the log messages with source code information because that's useful during debugging and I wanted to be able to send SMS
messages about some log events.

**J4JLogging** is my approach to doing both those things. It's a simple wrapper for Serilog which makes it easy to include caller 
information, source code information and sending text messages via Twilio for selected log events.

Apologies for the sparse documentation. Another project I'm working on is a system for creating ReadTheDocs/Sphinx style 
documentation for C# projects. But it's not ready yet :).

### Breaking Changes (v1 -> v2)

I realized recently that using a generic IJ4JLogger<> interface was needlessly complicated because
the only reason the type needed to be specified was to extract the type name for annotating log
entries. Since the generic interface couldn't be easily passed between types the net result was
a significant complication in using the library.

So I converted everything over to a non-generic IJ4JLogger interface definition. Since the library
still needs to know what type it's logging events for to annotate the event properly I implemented
an IJ4JLoggerFactory interface which has a single method you use like this:

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
I made them *channels* which you add to J4JLogger. Each channel takes care of configuring
the underlying Serilog logger.

This approach also let me implement a Twilio logger in a more natural way. It's built on top
of a Serilog TextWriter sink so that all of the formatting provided by Serilog for log events
comes in for free.

I apologize for not maintaining a final release of v1. But I ran into a problem I had to fix
which greatly exceeded my git command line skills, the net result of which was that I had to
blow away this repository and recreate it from a working v2.

### Configuration

I typically use J4JLogger via dependency injection, including a constructor argument wherever I want to log stuff:

```csharp
    private readonly IJ4JLogger _logger;

    public Simulator( IJ4JLoggerFactory loggerFactory )
    {
        _logger = loggerFactory?.CreateLogger( typeof(Simulator) ) ??
                          throw new NullReferenceException( nameof(loggerFactory) );
    }
```

To create the IJ4JLoggerFactory I typically use dependency injection via [Autofac](https://autofac.org/), 
another package that I highly recommend you consider. In the Autofac environment configuring J4JLogger 
can be done like this inside of a Module.Load() override:

```csharp
builder.Register<J4JLoggerConfiguration>( ( c, p ) =>
{
    var configFilePath = Path.Combine( Environment.CurrentDirectory, "J4JLoggingTest.json" );

    var channelTypes = new Dictionary<LogChannel, Type>()
    {
        { LogChannel.Console, typeof(LogConsoleConfiguration) },
        { LogChannel.Debug, typeof(LogDebugConfiguration) },
        { LogChannel.File, typeof(LogFileConfiguration) },
        { LogChannel.TextWriter, typeof(LogTwilioConfiguration) },
    };

    return J4JLoggerConfiguration.CreateFromFile<J4JLoggerConfiguration>( 
        configFilePath,
        channelTypes );
} )
    .As<IJ4JLoggerConfiguration>()
    .SingleInstance();

builder.RegisterAssemblyTypes( typeof(J4JLoggingModule).Assembly )
    .Where( t => typeof(LogChannelConfiguration).IsAssignableFrom( t )
         && !t.IsAbstract
         && ( t.GetConstructors()?.Length > 0 ) )
    .AsImplementedInterfaces()
    .SingleInstance();

builder.Register((c, p) =>
{
    var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();
    return loggerConfig.CreateLogger();
})
    .As<ILogger>()
    .SingleInstance();

builder.RegisterType<J4JLoggerFactory>()
    .As<IJ4JLoggerFactory>()
    .SingleInstance();
```

The first code block tells Autofac how to create instances of whatever configuration object you're using
which implements IJ4JLoggerConfiguration. In this example that's an instance of J4JLoggerConfiguration
itself. But it could be a class derived from J4JLoggerConfiguration or, with a bit of additional
work, a class which contains an instance of J4JLoggerConfiguration.

The ultimate source of information for the configuration object in this example is a JSON configuration
file. You could, of course, define the configuration object explicitly. But I tend to put it in
a JSON file so I can tweak it at runtime without having to recompile the program.

It's in this first block where the logging channels (e.g., console, debug) the logger will support
are defined. These are held in a dictionary keyed by an enum describing the channel which contains
the types of channels being used. This information is needed to configure the JSON parser that will
create the configuration object.

The second code block is needed so that Autofac can resolve the requests for the various
channel objects needed by the first code block. In this example they're all within a single assembly
but you could create a custom on stored elsewhere and include it in the registration process.

The third code block registers the construction logic for the Serilog ILogger instance underlying
whatever IJ4JLoggers get created. 

The last block registers an IJ4JLoggerFactory which is used in your code's constructor to create
IJ4JLogger instances for each class for which you want to implement logging.

Note that all of these blocks constrain Autofac to create only single instances of the various
objects. That may not be strictly necessary but it seems like the right thingn to do with an
approach which leads to creating a logger factory. 

Also, the CreateFromFile() helper method hides a lot of detailed setup work. If you want to tweak
your specific setup process you should study how it works. In a similar vein all but the first
code block in this example are contained in an Autofac module you can include with a single
line in your ServiceProvider class:

```csharp
builder.RegisterModule<J4JLoggingModule>();
```

### Usage

Using J4JLogger is very similar to using Serilog since it's essentially just a wrapper around an `ILogger` instance. You can use any
of Serilog's basic logging method calls, Write, Debug, Error, Information, Verbose and Warning.

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

The Write methods are similar, except they each start off with a Serilog `LogEventLevel` argument which specifies the log event level.

**There is one significant difference in how you call these methods from the Serilog standard, however.** If you pass anything other than a simple string (i.e., a value for the template argument) to the methods you must specify the types of the propertyValue arguments explicitly in the method call. An example:

```csharp
int someIntValue = 1;
_logger.Debug<int>("The value of that argument is {someIntValue}", someIntValue);
```

This requirement comes about because the `memberName`, `srcPath` and `srcLine` arguments are automagically set for you by the
compiler and the fact that `memberName` and `srcPath` are strings "collides" with the `template` argument.

Happy logging!
