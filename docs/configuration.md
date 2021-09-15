# Configuration

Configuring `J4JLogger` is simple because, since it wraps `Serilog`, configuring it mostly means
configuring `Serilog` the way you always have. Here's an example:

```csharp
var loggerConfig = new J4JLoggerConfiguration()
    {
        CallingContextToText = ConvertCallingContextToText
    }
    .AddEnricher<CallingContextEnricher>();

loggerConfig.SerilogConfiguration
    .WriteTo.Debug()
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine( Environment.CurrentDirectory, "log.txt" ),
        rollingInterval: RollingInterval.Day );

var logger = loggerConfig.CreateLogger();
logger.SetLoggedType( typeof(Program) );
```

The `SerilogConfiguration` property on the `J4JLoggerConfiguration` instance gives you access to
the traditional `Serilog::LoggerConfiguration` API.

You can also use the cool `Serilog` `IConfiguration`-based API, like this:

```csharp
var configRoot = new ConfigurationBuilder()
    .AddJsonFile( Path.Combine( Environment.CurrentDirectory, "appConfig.json" ), true )
    .Build();

loggerConfig.SerilogConfiguration
    .ReadFrom
    .Configuration( configRoot );
```

Your JSON configuration file will have to follow the required `Serilog` configuration syntax, but it's 
pretty straightforward:

```json
  "Serilog": {
    "MinimumLevel": "Information",
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Debug" ],
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {CallingContext} {Exception}{NewLine}"
        }
      },
      {
        "Name": "Debug",
        "Args": {
          "outputTemplate": "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {CallingContext} {Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "log.txt",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {CallingContext} {Exception}{NewLine}"
        }
      }
    ]
  }
```

setting global defaults for shared configurtation properties (e.g., like `MinimumLevel`). Global
defaults, set on instances of `J4JLogger`, define the values used by each channel unless you
override them on a channel-by-channel basis.

This can be extremely simple if the defaults are acceptable:
```csharp
var logger = new J4JSoftware.Logging.J4JLogger();
logger.AddDebug();
logger.AddConsole();
logger.AddFile();
```
That code will log events to the debug output window, the console window and to a log file stored
in `Environment.CurrentDirectory` that rolls over once a day. All log events will be output (i.e.,
the minimum threshold is set to *Verbose*).

But you can do much more configuration if you want to (and you'll generally want to :)). 
#### Configuring by Applying Configuration Values
The next easiest way of setting up J4JLogger is to apply configuration values to either the J4JLogger
instance or one or more channels. Here's an example of the former:
```csharp
var globalConfig = new ChannelConfiguration
    {
        IncludeSourcePath = true,
        SourceRootPath = "c:/the root/of some/vs project",
        RequireNewLine = true,
        MinimumLevel = LogEventLevel.Debug
    };

logger.ConfigureLogger( globalConfig );
```
This sets certain global parameters for J4JLogger. Recall that these global parameters apply to 
every logging channel except for when the logging channel has a value set for the same
parameter. Any property who value you don't set when creating an instance of `ChannelConfiguration`
is null, and ignored by the `ConfigureLogger()` extension method.

There's a corresponding method for channels as well, `ConfigureChannel()`. It also takes an instance of
`ChannelConfiguration`.

The more specialized channels `FileChannel` and `TwilioChannel` have corresponding extension methods
(`ConfigureFileChannel()` and `ConfigureTwilioChannel()`, respectively).

#### Configuring based on an IConfiguration Source
It's common to want the logging service to be configured on an application-wide basis. This makes
it a natural for being associated with dependency injection and defining a composition root for
an application. While you can certainly extract the information you need to create instances of 
the configuration objects discussed above from configuration files in lots of different ways, 
there's a great fit between this goal and Microsoft's `IHost` API.

So I added some cool (IMHO) features to J4JLogger in the code for `J4JCompositionRoot`, which you
can [read about in detail at it's GitHub page](https://github.com/markolbert/ProgrammingUtilities/blob/master/docs/dependency.md).
But here's a brief overview.

You define your own composition root class by deriving from a base class (currently
either `J4JCompositionRoot` or `XAMLCompositionRoot`; the former is targeted at console applications
while the latter is targeted at WPF applications). For a console app that might look like this:
```csharp
public class CompositionRoot : J4JCompositionRoot
{
    public CompositionRoot()
        : base(
            "J4JSoftware",
            Program.AppName,
            "J4JSoftware.GeoProcessor.DataProtection",
            typeof(AppConfig)
        )
    {
        UseConsoleLifetime = true;
    }

    protected override void ConfigureLogger( J4JLogger logger )
    {
        if( LoggerConfiguration is not AppConfig appConfig )
            return;

        LoggerConfigurator.Configure( logger, appConfig.Logging );
    }
}
```
The `typeof(AppConfig)` parameter in the `base()` call defines an application object 
which holds configuration information for the logging system. It can be whatever you want it to
be. But if you define it to include a `LoggerInfo` property (`LoggerInfo` is defined in the J4JLogger
library) then when the `IConfiguration` system does its stuff you'll end up with a single
parameter you can pass to `LoggerConfigurator.Configure()` to take care of configuring `J4JLogger`.

`LoggerConfigurator` is defined in the `J4JLogger` library. It handles all of the basic channels 
defined in the library (i.e., console, debug, file, last event, net event). There's a derived class,
`TwilioLoggerConfigurator`, defined in the Twilio extension library, which adds handling for a Twilio
channel. You change logger configurators by passing an instance of whichever one you want into the
`J4JCompositionRoot` constructor call (if you don't pass anything you get a `LoggerConfigurator`).

The configuration file syntax for logging information looks like this:
```
  "Logging": {
    "Global": {
      "SourceRootPath": "C:\\Programming\\GeoProcessor\\",
      "IncludeSourcePath":  true 
    },
    "Channels":[
      "Console"
    ],
    "ChannelSpecific": {
      "Debug": {
        "MinimumLevel": "Information"
      }
    }
  }
```
There's a global section holding the default parameters and a channel-specific section holding
parameters for individual channels. The *Channels* section is for including channels where the default
channel values are all you want (empty entries in the channel-specific area are ignored by the 
`IComposition` API so you can't just add a blank entry there). 

You can also add to the `LoggingInfo` data by adding a list of channel names to the 
`LoggerConfigurator.Configure()` call. The corresponding channels will be set up with default values.