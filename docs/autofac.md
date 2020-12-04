### Autofac Support

Because I love the [Autofac dependency injection framework](https://autofac.org/) 
I've provided some extension methods to simplify setting up J4JLogging with 
`Autofac`. They're contained in an addon library.

Here's an example when the logging configuration info is in a JSON file 
defining a class implementing `IJ4JLoggerConfiguration` (a *derived* 
configuration):

```csharp
var configBuilder = new ConfigurationBuilder();

var config = configBuilder
    .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "logConfig.json"))
    .Build();

var builder = new ContainerBuilder();

var channelConfig = config.GetSection( "Channels" ).Get<ChannelConfiguration>();

builder.Register( c =>
{
    var retVal = config.GetSection( "Logging" ).Get<J4JLoggerConfiguration<ChannelConfiguration>>();

    retVal.Channels = channelConfig;

    return retVal;
} )
    .As<IJ4JLoggerConfiguration>()
    .SingleInstance();

builder.RegisterJ4JLogging();
```
In order for `RegisterJ4JLogging()` to work an instance of `IJ4JLoggerConfiguration` must be resolvable 
(i.e., able to be created by Autofac). The class `J4JLoggerConfiguration<TChannels>` implements `IJ4JLoggerConfiguration`,
but to create an instance of it you need to be specify an instance of a class which implements
`ILogChannels`. In this example that's done by extracting an instance of `ChannelConfiguration`,
which implements `ILogChannels`, from the `IConfigurationRoot` system, but you could create it some other way.