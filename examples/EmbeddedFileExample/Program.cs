using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoFacJ4JLogging;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#pragma warning disable 8618

namespace J4JLogger.Examples
{
    // shows how to use the J4JLogger with a configuration file where the logger
    // configuration information is contained in a child property within a larger
    // JSON configuration file.
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

            // we use Microsoft's ConfigurationBuilder to create an instance of a configuration
            // object derived from the JSON configuration file. The resulting IConfigurationRoot
            // is passed to a J4JLogger Autofac-based registration method.
            var configRoot = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("logConfig.json")
                .Build();

            // "Logger" is the key of the key/value pair within IConfigurationRoot which holds
            // the J4JLoggerConfiguration-based configuration information
            builder.AddJ4JLogging<J4JLoggerConfiguration>(
                configRoot,
                "Logger",
                typeof(ConsoleChannel), typeof(DebugChannel), typeof(FileChannel)
            );

            _svcProvider = new AutofacServiceProvider( builder.Build() );
        }
    }
}
