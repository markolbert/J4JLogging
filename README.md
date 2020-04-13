# J4JLogging

I'm a huge fan of [Serilog](https://serilog.net/) and use it in all of my C# work. But I wanted to be able to annotate 
the log messages with source code information because that's useful during debugging and I wanted to be able to send SMS
messages about some log events.

**J4JLogging** is my approach to doing both those things. It's a simple wrapper for Serilog which makes it easy to include caller 
information, source code information and sending text messages via Twilio for selected log events.

Apologies for the sparse documentation. Another project I'm working on is a system for creating ReadTheDocs/Sphinx style 
documentation for C# projects. But it's not ready yet :).

### Configuration

I typically use J4JLogger via dependency injection, including a constructor argument wherever I want to log stuff:

```csharp
public DataRetriever( 
    FppcFilingContext dbContext,
    IJ4JLogger<DataRetriever> logger )
    {
```

Notice that the I4JLogger interface is generic, with the type argument being the type of the object which will be doing the logging.
That's needed to support the kind of source code logging J4JLogger is designed to do.

Creating instances of J4JLogger involves passing instances of a Serilog logger and a J4JLoggerConfiguration object to the J4JLogger
constructor:

```csharp
public J4JLogger(
    ILogger logger,
    IJ4JLoggerConfiguration config
```

I typically do this via dependency injection generally using [Autofac](https://autofac.org/), another package that I highly 
recommend you consider. In the Autofac environment configuring J4JLogger can be done like this:

```csharp
builder.Register<FppcFilingConfiguration>( ( c, p ) =>
    {
        var retVal = new ConfigurationBuilder()
            .SetBasePath( Environment.CurrentDirectory )
            .AddUserSecrets<Program>()
            .AddJsonFile( "configInfo.json" )
            .Build()
            .Get<FppcFilingConfiguration>();

        return retVal;
    } )
    .AsSelf()
    .SingleInstance();

builder.Register<IJ4JLoggerConfiguration>( ( c, p ) => c.Resolve<FppcFilingConfiguration>().Logger );

builder.Register<ILogger>( ( c, p ) =>
    {
        var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();

        return new LoggerConfiguration()
            .Enrich.FromLogContext()
            .SetMinimumLevel( loggerConfig.MinLogLevel )
            .WriteTo.Console( restrictedToMinimumLevel: loggerConfig.MinLogLevel )
            .WriteTo.File(
                path: J4JLoggingExtensions.DefineLocalAppDataLogPath( "log.txt", "J4JSoftware/AlphaVantageRetriever" ),
                restrictedToMinimumLevel: loggerConfig.MinLogLevel
            )
            .CreateLogger();
    } )
    .SingleInstance();

builder.RegisterGeneric( typeof( J4JLogger<> ) )
    .As( typeof( IJ4JLogger<> ) )
    .SingleInstance();
```

The first code block tells Autofac how to create instances of a configuration object which contains, as a property called
`Logger`, the J4JLogger configuration object. You don't have to embed the `J4JLoggerConfiguration` object inside another
object, of course; I just often do that because I find it easier to embed the `J4JLoggerConfiguration` information in the
app's master configuration file.

The second, one line, code block tells Autofac to resolve requests for `IJ4JLoggerConfiguration` objects from the `Logger`
property of my master configuration object.

The third code block describes for Autofac how I want the underlying Serilog `ILogger` object to be configured. The details here
will depend on how and where you want to log. In this case I'm allowing the minimum log level to be set from the configuration
file and logging to both the console and a file. The log file will be located in C:\Users\Mark\AppData\Local\J4JSoftware\AlphaVantageRetriever (because Windows encourages you to not write log files to an app's
home directory).

**The `Enrich.FromLogContext()` line is necessary for J4JLogger to work its magic.**

The final code block tells Autofac to create single instances of each generic IJ4JLogger<> object. Those are the objects that are
injected into the constructors of the objects where you're doing logging.

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
