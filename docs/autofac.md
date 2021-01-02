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

var factory = new ChannelFactory( config );

factory.AddChannel<ConsoleConfig>( "channels:console" );
factory.AddChannel<DebugConfig>( "channels:debug" );
factory.AddChannel<FileConfig>( "channels:file" );

builder.RegisterJ4JLogging<DerivedConfiguration>( factory );

_svcProvider = new AutofacServiceProvider( builder.Build() );
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
The specific logging channels are specified when the channel factory (`factory`) 
is set up. The string arguments to each `Add<>()` call are the paths, relative to
the root of the json file, to the sections which configure the specified type
of channel. The paths are not case sensitive, as per the JSON spec.