# J4JLogging

A Net 5.0 library which wraps [Serilog's ILogger](https://github.com/serilog/serilog) and extends it by
reporting source code information.

[![Nuget](https://img.shields.io/nuget/v/J4JSoftware.Logging?style=flat-square)](https://www.nuget.org/packages/J4JSoftware.Logging/)


### TL;DR

```csharp
using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#pragma warning disable 8618

namespace J4JLogger.Examples
{
    // shows how to use the J4JLogger with a simple configuration file containing
    // nothing but logger configuration information.
    class Program
    {
        private static IServiceProvider _svcProvider;

        static void Main(string[] args)
        {
            InitializeServiceProvider();

            var logger = _svcProvider.GetRequiredService<IJ4JLogger>();
            logger.SetLoggedType<Program>();

            logger.Information("This is an Informational logging message");
            logger.Fatal("This is a Fatal logging message");
        }

        private static void InitializeServiceProvider()
        {
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
        }
    }

    public class ChannelConfiguration : LogChannels
    {
        public ConsoleConfig Console { get; set; }
        public DebugConfig Debug { get; set; }
        public FileConfig File { get; set; }

        public override IEnumerator<IChannelConfig> GetEnumerator()
        {
            yield return Console;
            yield return Debug;
            yield return File;
        }
    }
}

Contents of logConfig.json:
{
  "DefaultElements": "SourceCode",
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
### Important Note
**There is one significant difference in how you call the logging methods
from the Serilog standard.** 

If you pass a simple string (i.e., a value for the template argument) to the methods you **must** specify the types of 
the propertyValue arguments explicitly in the method call. 

An example:

```csharp
string someStringValue = "abcd";
_logger.Debug<string>("The value of that argument is {someIntValue}", someStringValue);
```
This requirement comes about because the `memberName`, `srcPath` and `srcLine` 
arguments are automagically set for you by the compiler. The fact the 
`memberName` and `srcPath` arguments of the logging methods are strings and
"collide" string arguments you may specify. That makes explict type 
specifications for the arguments necessary when strings are referenced by the message template.

### Table of Contents

- [Goal and Concept](docs/goal-concept.md)
- [Terminology](docs/terminology.md)
- [Usage](docs/usage.md)
- [Configuration](docs/configuration.md)
- [Specifying Event Elements to Include](docs/elements.md)
- [Autofac Dependency Injection Support](docs/autofac.md)
- [The Twilio channel](docs/twilio.md)
- [Adding a channel](docs/channel.md)

#### Inspiration and Dedication

I'm a huge fan of [Serilog](https://serilog.net/) and use it in all of my 
C# work. This library is dedicated to Nicholas Blumhardt and the rest of
the Serilog team.

But I wanted to be able to annotate the log messages with source code information because that's 
useful during debugging and I wanted to be able to send SMS messages about some log events.

**J4JLogging** is my approach to doing both those things. It's a simple wrapper for Serilog 
which makes it easy to include caller information, source code information and sending text 
messages via Twilio for selected log events.

Apologies for the sparse documentation. Another project I'm working on is a system for 
creating ReadTheDocs/Sphinx style documentation for C# projects. But it's not ready yet :).
