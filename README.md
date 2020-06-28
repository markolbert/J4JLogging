# J4JLogging

A Net Core 3.1 library which wraps [Serilog's ILogger](https://github.com/serilog/serilog) and extends it by
reporting source code information.

### TL;DR

```csharp
using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoFacJ4JLogging;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;

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
        }

        private static void InitializeServiceProvider()
        {
            var builder = new ContainerBuilder();

            builder.AddJ4JLogging<J4JLoggerConfiguration>(
                Path.Combine( Environment.CurrentDirectory, "logConfig.json" ),
                typeof(ConsoleChannel),
                typeof(DebugChannel),
                typeof(FileChannel) );

            _svcProvider = new AutofacServiceProvider( builder.Build() );
        }
    }
}
```

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
