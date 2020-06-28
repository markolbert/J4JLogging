### Configuration

To simplify creating `IJ4JLoggerConfiguration` instances the library includes
configuration builders. These can be used standalone or with an addon to
the library which [provides Autofac support](autofac.md).

One of the builders works directly with JSON-formatted text. The other works
with Microsoft's `ConfigurationBuilder` framework and revolves around
`IConfigurationRoot`. 

The first builder is intended for situations where the configuration 
information is in a JSON-file whose structure maps to `J4JLoggerConfiguration` 
or a class derived from `J4JLoggerConfiguration`. I call these *derived
configurations*. Here's an example of such a configuration file:
```json
{
  "SomeOtherProperty": true,
  "SomeOtherArray": [
    "a",
    "b",
    "c"
  ],
  "SomeOtherObject" : {
    "Property1": 15,
    "Property2": "abc" 
  },
  "SourceRootPath": "C:/Programming/J4JLogging/",
  "Channels": [
    {
      "Channel": "Console",
      "MinimumLevel": "Verbose"
    },
    {
      "Channel": "Debug",
      "MinimumLevel": "Verbose"
    },
    {
      "Channel": "File",
      "Location": "AppData",
      "RollingInterval": "Day",
      "FileName": "log.text",
      "MinimumLevel": "Verbose"
    }
  ]
}
```
The information of interest to the **J4JLogger** framework is the content
starting with "SourceRootPath".

The second builder is used when the logging information is part of a larger
configuration file but the file's structure doesn't fit such a mapping. I
call these 'embedded configurations'. Here's an example:
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
    "EventElements" :  "SourceCode, Type", 
    "Channels": [
      {
        "Channel": "Console",
        "MinimumLevel": "Verbose"
      },
      {
        "Channel": "Debug",
        "MinimumLevel": "Verbose"
      },
      {
        "Channel": "File",
        "Location": "AppData",
        "RollingInterval": "Day",
        "FileName": "log.text",
        "MinimumLevel": "Verbose"
      }
    ]
  }
}
```
The information of interest to the **J4JLogger** framework is the content
contained with the "Logger" node.

#### Derived Configuration Builder

The derived configuration builder, `J4JLoggerConfigurationJsonBuilder` is 
used like this:
```csharp
var configBuilder = new J4JLoggerConfigurationJsonBuilder();

foreach( var kvp in channels )
{
    configBuilder.AddChannel( kvp.Value );
}

configBuilder.FromFile( configFilePath );

return configBuilder.Build<TConfig>();
```
where `TConfig` is the class implementing `IJ4JLoggerConfiguration`. It can
contain additional fields to be mapped by the JSON deserializer. But it has
to implement `IJ4JLoggerConfiguration`. The return value is an instance of 
`TConfig`.

As of today no checking is done in the `FromFile()` call, either to see that
the file is a readable text file or if its content is validly-formatted
JSON.

#### Embedded Configuration Builder

That embedded configuration builder, `J4JLoggerConfigurationRootBuilder` 
is used like this:

```csharp
var configRoot = new ConfigurationBuilder()
    .SetBasePath(Environment.CurrentDirectory)
    .AddJsonFile("logConfig.json")
    .Build();

var loggerBuilder = new J4JLoggerConfigurationRootBuilder();

// note: channelTypes is an enumerable collection of types 
// implementing ILogChannel and decorated with a ChannelAttribute
// which specifies the channel's unique name (e.g., console).
foreach( var channelType in channelTypes )
{
    loggerBuilder.AddChannel( channelType );
}

// the second argument in this next call is the name of the property holding 
// the logging configuration information. In the example configuration file 
// shown earlier this would be "Logger"
return loggerBuilder.Build<TConfig>( configRoot, "Logger" );
```
The last line returns an instance of the class implementing 
`IJ4JLoggingConfiguration`. Typically this would be `J4JLoggingConfiguration` 
itself.
