### Configuration

Configuring `IJ4JLogger` is a matter of creating an instance of 
`J4JLoggerConfiguration`. How you do that is up to you, but if you use the Net5 
configuration system the `ChannelFactory` class simplifies things a lot:
```csharp
var config = new ConfigurationBuilder()
    .AddJsonFile( Path.Combine( Environment.CurrentDirectory, "logConfig.json" ) )
    .Build();

var builder = new ContainerBuilder();

var factory = new ChannelFactory( config );

factory.AddChannel<ConsoleConfig>( "channels:console" );
factory.AddChannel<DebugConfig>( "channels:debug" );
factory.AddChannel<FileConfig>( "channels:file" );

builder.RegisterJ4JLogging<DerivedConfiguration>( factory );

_svcProvider = new AutofacServiceProvider( builder.Build() );
```
All you do is create an instance of `ChannelFactory` and tell it how to 
locate each of the configuration sections which define the channels you want to
use. 

The configuration paths are those used in the Net5 configuration system: class or
property names separated by colons, all relative to the root of the JSON file. The
paths are case-insensitive.

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
  "SourceRootPath": "C:/Programming/J4JLogging/",
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
```
If instead your configuration file looked like this (where the logging information
is embedded within a property of the configuration object):
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
```
your configuration code would look like this:
```csharp
var config = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "logConfig.json"))
    .Build();

var builder = new ContainerBuilder();

var channelFactory = new ChannelFactory( config, "Logger" );

channelFactory.AddChannel<ConsoleConfig>( "Logger:Channels:Console" );
channelFactory.AddChannel<DebugConfig>("Logger:Channels:Debug");
channelFactory.AddChannel<FileConfig>("Logger:Channels:File");

builder.RegisterJ4JLogging<J4JLoggerConfiguration>( channelFactory );

_svcProvider = new AutofacServiceProvider(builder.Build());
```
The only changes were to 
- specify the "root" of the logger configuration is found in the section 
 named "Logger"; and,
- the individual channel configuration sections are all under the section "Logger"

The `ChannelFactory` constructor also allows you to set a flag to enable the 
**LastEvent** channel. This is a special channel which holds the most recent log
event sent through the logging system. I use it mostly in automated tests, to ensure
what comes out is what should be coming out from a given log event.