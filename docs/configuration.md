# Configuration

Configuring `J4JLogger` is simple because, since it wraps `Serilog`, configuring it mostly means configuring `Serilog` the way you always have:

```csharp
// the code for the constructor parameter is omitted for clarity/brevity.
// see the docs on trimming paths for more information.
var loggerConfig = new J4JLoggerConfiguration( FilePathTrimmer );

var outputTemplate = loggerConfig.GetOutputTemplate( true );

loggerConfig.SerilogConfiguration
    .WriteTo.Debug( outputTemplate: outputTemplate )
    .WriteTo.Console( outputTemplate: outputTemplate )
    .WriteTo.File(
        path: Path.Combine( Environment.CurrentDirectory, "log.txt" ),
        outputTemplate: outputTemplate,
        rollingInterval: RollingInterval.Day );

var logger = loggerConfig.CreateLogger();
logger.SetLoggedType( typeof(Program) );
```

The `SerilogConfiguration` property on the `J4JLoggerConfiguration` instance gives you access to the traditional `Serilog::LoggerConfiguration` API.

You can also use the cool `Serilog` `IConfiguration`-based API, like this:

```csharp
var configRoot = new ConfigurationBuilder()
    .AddJsonFile( Path.Combine( Environment.CurrentDirectory, "appConfig.json" ), true )
    .Build();

loggerConfig.SerilogConfiguration
    .ReadFrom
    .Configuration( configRoot );
```

For more information on how to define the JSON configuration file refer to [this documentation](docs/iconfig-based.md).
