### Configuration

Configuring `IJ4JLogger` is a matter of creating an instance of `J4JLoggerConfiguration<TChannels>`.
How you do that is up to you, although I typically derive it from the Net5 configuration
system (this example uses methods from the add-on Autofac support library, but our
focus here is on how the configuration is created):
```csharp
var configBuilder = new ConfigurationBuilder();

var config = configBuilder
    .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "logConfig.json"))
    .Build();

var builder = new ContainerBuilder();

var channelConfig = config.GetSection("Channels").Get<ChannelConfiguration>();

builder.Register(c =>
    {
        var retVal = config.Get<J4JLoggerConfiguration<ChannelConfiguration>>();

        retVal.Channels = channelConfig;

        return retVal;
    } )
    .As<IJ4JLoggerConfiguration>()
    .SingleInstance();

builder.RegisterJ4JLogging();

_svcProvider = new AutofacServiceProvider(builder.Build());
```
Creating an instance of `J4JLoggerConfiguration<TChannels>` involves creating it
(done here using Microsoft's configuration system) **and** setting the value of
its `Channels` property. The `Channels` property holds the configuration information
for every channel/sink used by `IJ4JLogger`, and knows how to configure each of
those sinks when called upon to do so by the initialization code.

The reason two separate calls into the configuration system are done -- one to 
initialize an instance of `ChannelConfiguration` and one to initialize an
instance of `J4JLoggerConfiguration<ChannelConfiguration>` -- is because I didn't
want to hard code what you call the "channels" section of your configuration file,
or where it's located within the overall configuration structure. 
