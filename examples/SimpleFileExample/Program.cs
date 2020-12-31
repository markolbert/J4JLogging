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

            builder.RegisterJ4JLogging( config );

            _svcProvider = new AutofacServiceProvider(builder.Build());
        }
    }
}
