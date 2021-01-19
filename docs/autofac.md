### Autofac Support

Because I love the [Autofac dependency injection framework](https://autofac.org/) 
I've provided some extension methods to simplify setting up J4JLogging with 
`Autofac`. They're contained in an addon library.

Here's an example when the logging configuration info is in a JSON file 
defining a class implementing `IJ4JLoggerConfiguration` (a *derived* 
configuration):

```csharp
var config = new ConfigurationBuilder()
    .AddJsonFile( Path.Combine( Environment.CurrentDirectory, "logConfig.json" ) )
    .Build();

var builder = new ContainerBuilder();

var provider = new ChannelConfigProvider
    {
        Source = config
    }
    .AddChannel<ConsoleConfig>("channels:console")
    .AddChannel<DebugConfig>("channels:debug")
    .AddChannel<FileConfig>("channels:file");

builder.RegisterJ4JLogging<J4JLoggerConfiguration>( provider );

_svcProvider = new AutofacServiceProvider(builder.Build());
```

This assumes the `logConfig.json` file is structured with all the necessary
`J4JLogging` properties at the root level:
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
The logging channels are specified by adding them to the provider along
with the `IConfiguration` property path. Those property paths are 
relative to location in the json file specified by the property path
provided when the `DynamicChannelConfigProvider` instance is created. 

In the example no such argument is given so the property paths in the
`AddChannel<>()` calls are relative to the root of the file. Had the
json file been structured like this (an **embedded** configuration):
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
the provider setup would have looked like this:
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
Note the argument **"Logger"** in the constructor call. It makes
the property paths relative to the **Logger** section of the json file.

Property paths are case-insensitive, as per the JSON spec.