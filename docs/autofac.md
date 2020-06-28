### Autofac Support

Because I love the [Autofac dependency injection framework](https://autofac.org/) 
I've provided some extension methods to simplify setting up J4JLogging with 
`Autofac`. They're contained in an addon library.

Here's an example when the logging configuration info is in a JSON file 
defining a class implementing `IJ4JLoggerConfiguration` (a *derived* 
configuration):

```csharp
var containerBuilder = new ContainerBuilder();

containerBuilder.AddJ4JLogging<TConfig>(
    configFilePath,
    typeof(ConsoleChannel),
    typeof(DebugChannel),
    typeof(FileChannel),
    typeof(TwilioChannel)
);
```
`TConfig` is a class implementing `IJ4JLoggerConfiguration` and `configFilePath` is
a string defining the location of the JSON configuration file. You can add as many
channels as you wish.

When the logging configuration info is in a property of a JSON file which will be
used with the `Microsoft.Extensions.Configuration` API (an *embedded* 
configuration) you'd use a pattern like this:
```csharp
var builder = new ContainerBuilder();

var configRoot = new ConfigurationBuilder()
    .AddJsonFile( configFilePath )
    .Build();

builder.Register(c => configRoot.Get<Configuration>())
    .AsSelf()
    .SingleInstance();

builder.AddJ4JLogging<J4JLoggerConfiguration>(
    configRoot,
    "Logger", 
    typeof(ConsoleChannel), typeof(FileChannel) );
```
You don't need to register the overall configuration object (`Configuration` 
in this example) unless you want to access it via dependency injection. 
You also can add as many channels as you wish.

The `Autofac` extension methods create singleton instances of 
`IJ4JLoggerConfiguration` and `ILogger` (the underlying `Serilog` logger). 
That's the pattern I typically use but your use case may be different.
