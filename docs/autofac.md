### Autofac Support

Because I love the [Autofac dependency injection framework](https://autofac.org/) 
I've provided some extension methods to simplify setting up J4JLogging with 
`Autofac`. They're contained in an addon library.

Here's an example when the logging configuration info is in a JSON file 
defining a class implementing `IJ4JLoggerConfiguration` (a *derived* 
configuration):

```csharp
var config = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "logConfig.json"))
    .Build();

var builder = new ContainerBuilder();

builder.RegisterJ4JLogging(config);

_svcProvider = new AutofacServiceProvider(builder.Build());
```

This assumes the `logConfig.json` file is structured with all the necessary
`J4JLogging` information contained in a section labeled "Logging":
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
  "Logging": {
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
}
```
This particular `Autofac` method call limits you to using the channels defined
in the base assembly (Console, Debug, File, Twilio and LastEvent) but lets you
easily configure a number of logging attributes:
```csharp
public static ContainerBuilder RegisterJ4JLogging(
    this ContainerBuilder builder, 
    IConfigurationRoot config, 
    string? logKey = null, 
    AvailableChannels channels = AvailableChannels.All,
    EventElements defaultElements = EventElements.All,
    LogEventLevel minLevel = LogEventLevel.Verbose,
    string outputTemplate = ChannelConfig.DefaultOutputTemplate,
    TwilioConfig? twilioConfig = null )
    {
    ...
    }
```
- `logKey` is the name of the section containing the logging configuration information.
If it's empty or null (the default) it assumes the overall configuration object
you're constructing from the json file is derived from 
`J4JLoggerConfiguration<ChannelConfiguration>`.
- `channels` 