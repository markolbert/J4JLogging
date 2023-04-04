# J4JSoftware.Logging

**As of 2023 April 4 I am no longer developing this library.**

The reason is simple: I learned more about how [Serilog](https://github.com/serilog/serilog) works and realized there was a
much easier way of implementing the same functionality :).

Take a look at [J4JLoggerEnhancements](https://github.com/markolbert/J4JLoggerEnhancements) for a new, slimmer approach for doing what **J4JLogging** does.

-----

|Version|Description|
|:-----:|-----------|
|4.2.1|Updated packages, **announced end of development**|
|4.2.0|Updated to Net 7, updated packages|
|4.1.0|Updated to Net 6, updated packages, Changed how `NetEventArg` objects are created|
|4.0.0|**Major breaking changes**, [see details below](#400)|
|3.2.0|Moved the Twilio SMS stuff into a separate assembly because it's not commonly used when logging and is large relative to the rest of the code base. The libraries are now licensed under the GNU GPL-v3.0 (or later) license.|
|3.1.0|[See details below](#310)|
|3.0.0|[See details below](#300)|

## 4.0.0

Earlier versions of `J4JLogger` tried to "simplify" defining and configuring channels/sinks. However, as I continued to use the library -- and learned more about `Serilog` -- I came to realize a better approach was to stick as closely as possible to the "Serilog way" of doing things. Because having to remember two similar-but-different configuration approaches quickly gets confusing.

To that end, the entire concept of channels (which were related to sinks) is gone. Instead, you configure  `J4JLogger`-specific features (there's currently only one, a facility for sending log events as text messages via SMS) through a configuration object and do everything else the way you always have with `Serilog`, by using its `LoggerConfiguration` API.

Doing that requires you configure the `LoggerConfiguration` instance embedded within an instance of `J4JLoggerConfiguration`. That's done like this:

```csharp
var loggerConfig = new J4JLoggerConfiguration();

var outputTemplate = loggerConfig.GetOutputTemplate( true );

loggerConfig.SerilogConfiguration
    .WriteTo.Debug( outputTemplate: outputTemplate )
    .WriteTo.Console( outputTemplate: outputTemplate )
    .WriteTo.File(
        path: Path.Combine( Environment.CurrentDirectory, "log.txt" ),
        outputTemplate: loggerConfig.GetOutputTemplate( true ),
        rollingInterval: RollingInterval.Day );

var logger = loggerConfig.CreateLogger();
logger.SetLoggedType( typeof(Program) );
```

The `SerilogConfiguration` property on the `J4JLoggerConfiguration` instance gives you access to
the traditional `Serilog::LoggerConfiguration` API.

You may wonder why there's a call to `loggerConfig` to generate an output template to use in configuring the various `Serilog` sinks. The answer is simple: in order for the calling context information (e.g., calling method name) to appear in the log there has to be a placeholder for it the output template `Serilog` uses...and the default template obviously won't contain it.

I also eliminated the need to use dependency injection to configure the logger.

The `NetEventSink` is incorporated into the library in a different way. When it's included as a sink the events it generates are raised by `IJ4JLogger`. This simplified setting up the sink.

Adding `NetEventSink` is done by calling the extension method `NetEvent()` on an instance of `J4JLoggerConfiguration`:

```csharp
loggerConfig.NetEvent( outputTemplate: outputTemplate,
    restrictedToMinimumLevel: NetEventConfiguration.MinimumLevel );
```

Keep in mind that if you don't add it as a sink no `LogEvent`s will ever be raised by `IJ4JLogger`.

## 3.1.0

I've modified, once again, how the output channels are configured when initializing the logger.

You can set up a class implementing `IJ4JLoggerConfiguration` (e.g.,
`J4JLoggerConfiguration`) manually and add the channels you want to its `Channels` property.

Or, if you're using the Net5 `IConfiguration` system you can implement an instance of `IChannelConfigProvider` and use the Autofac registration methods to do the work for you. See the [configuration section](../docs/configuration.md) section for more details.

## 3.0.0

- The libraries now target Net5 only, and have null checking enabled.
- I consolidated all the default channels into the base J4JLogger assembly. Having them in separate assemblies complicated things.
- The way log channels are configured was changed substantially (mostly because even the author found the earlier approach difficult to remember :)).
- The `Autofac`-based setup approach was simplified.
- To make logging possible before a program is fully set up a cached implementation of IJ4JLogger was added. The contents of the cache can be easily dumped into the actual logging system once it's established.
