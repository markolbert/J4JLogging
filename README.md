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
            var config = new ConfigurationBuilder()
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, "logConfig.json" ) )
                .Build();

            var builder = new ContainerBuilder();

            var factory = new ChannelFactory( config );

            factory.AddChannel<ConsoleConfig>("channels:console");
            factory.AddChannel<DebugConfig>("channels:debug");
            factory.AddChannel<FileConfig>("channels:file");

            builder.RegisterJ4JLogging<J4JLoggerConfiguration>( factory );

            _svcProvider = new AutofacServiceProvider(builder.Build());
        }
    }
}
```
Contents of logConfig.json:
```json
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
### Significant Changes to v3
- The libraries now target Net5 only, and have null checking enabled.
- I consolidated all the default channels into the base J4JLogger assembly. Having
them in separate assemblies complicated things.
- The way log channels are configured was changed substantially (mostly because 
even the author found the earlier approach difficult to remember :)).
- The `Autofac`-based setup approach was simplified.
- To make logging possible before a program is fully set up a cached implementation
 of IJ4JLogger was added. The contents of the cache can be easily dumped into the actual
 logging system once it's established.
 
### Important Note
**There is one important difference in how you call the logging methods
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