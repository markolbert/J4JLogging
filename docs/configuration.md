### Configuration

Configuring `IJ4JLogger` is a matter of creating an instance of 
`J4JLoggerConfiguration`. How you do that is up to you, but if you use the 
Net5 `IConfiguration` system the `ChannelConfigProvider` class
simplifies things a lot:
```csharp
var builder = new ContainerBuilder();

var config = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "logConfig.json"))
    .Build();

var provider = new ChannelConfigProvider( "Logger" )
    {
        Source = config
    }
    .AddChannel<ConsoleConfig>( "channels:console" )
    .AddChannel<DebugConfig>( "channels:debug" )
    .AddChannel<FileConfig>( "channels:file" );

builder.RegisterJ4JLogging<J4JLoggerConfiguration>( provider );

_svcProvider = new AutofacServiceProvider(builder.Build());
```
All you do is create an instance of `ChannelConfigProvider` and 
tell it how to locate each of the configuration sections which define the 
channels you want to use. 

The configuration paths are those used in the Net5 configuration system: 
class or property names separated by colons, all relative to the section
specified in the `ChannelConfigProvider` constructor (in this 
example "Logger"). The paths are case-insensitive.

The above example is based on a JSON configuration file which looks like this:
```json
{
  "SomeOtherProperty": true,
  "SomeOtherArray": [
    "a",
    "b",
    "c"
  ],
  "SomeOtherObject": {
    "Property1": 15,
    "Property2": "abc"
  },
  "Logger": {
    "SourceRootPath": "C:/Programming/J4JLogging/",
    "EventElements": "SourceCode, Type",
    "Channels": {
      "Console": {
        "MinimumLevel": "Information"
      },
      "Debug": {
        "MinimumLevel": "Debug"
      },
      "File": {
        "Location": "AppData",
        "RollingInterval": "Day",
        "FileName": "log.text",
        "MinimumLevel": "Verbose"
      }
    }
  }
}

`ChannelConfigProvider` lets you to incorporate the `LastEventConfig` 
channel by setting the property `LastEvent` to an instance of 
`LastEventConfig`. If you do the provider will automatically make it 
available to the startup routines.

The `LastEventConfig` channel is a special one which holds the most 
recent log event sent through the logging system. I use it mostly in 
automated tests, to ensure what comes out is what should be coming 
out from a given log event.