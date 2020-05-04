using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoFacJ4JLogging;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace J4JLoggingTest
{
    class Program
    {
        static void Main( string[] args )
        {
            TestSerialization();

            RunTest<J4JLoggerConfiguration>( Path.Combine( Environment.CurrentDirectory, "J4JLoggingTest.json" ), "basic" );
            RunTest<DerivedConfiguration>( Path.Combine( Environment.CurrentDirectory, "J4JLoggingTest2.json" ), "derived" );
            RunTest<EmbeddedConfiguration>(Path.Combine(Environment.CurrentDirectory, "J4JLoggingTest3.json"), "free embedded");
        }

        private static void TestSerialization()
        {
            var channels = new List<LogChannel>
            {
                new ConsoleChannel(),
                new DebugChannel(),
                new FileChannel()
            };

            var config = new J4JLoggerConfiguration()
            {
                IncludeSource = false,
                IncludeAssemblyName = true,
                Channels = channels
            };

            var channelTypes = new Dictionary<ChannelAttribute, Type>()
            {
                { new ChannelAttribute("Console"), typeof(ConsoleChannel) },
                { new ChannelAttribute("Debug"), typeof(DebugChannel) },
                { new ChannelAttribute("File"), typeof(FileChannel) },
            };

            var builder = new J4JLoggerConfigurationJsonBuilder();

            builder.AddChannel<ConsoleChannel>()
                .AddChannel<DebugChannel>()
                .AddChannel<FileChannel>();

            var settings = builder.BuildSerializerSettings();
            settings.WriteIndented = true;

            var text = JsonSerializer.Serialize( config, settings );

            var check = JsonSerializer.Deserialize<J4JLoggerConfiguration>( text, settings );
        }

        private static void RunTest<TConfig>( string configFilePath, string label )
            where TConfig: class
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.AddJ4JLoggingFromFile<TConfig>(
                    configFilePath,
                    typeof(ConsoleChannel),
                    typeof(DebugChannel),
                    typeof(FileChannel),
                    typeof(TwilioChannel)
                )
                .Register( c => new TwilioTestConfig() )
                .As<ITwilioConfig>()
                .SingleInstance();

            // kludgy but it's just a test...
            if( typeof(TConfig) == typeof(EmbeddedConfiguration) )
            {
                containerBuilder.Register( c =>
                    {
                        var config = c.Resolve<EmbeddedConfiguration>();

                        return config.Logging;
                    } )
                    .As<IJ4JLoggerConfiguration>();
            }

            var services = new AutofacServiceProvider(containerBuilder.Build());

            var loggerFactory = services.GetRequiredService<IJ4JLoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(Program));

            var twilio = logger.Channels.FirstOrDefault(
                    c => c.Channel.Equals( "Twilio", StringComparison.OrdinalIgnoreCase ) )
                as TwilioChannel;

            twilio?.Initialize(services.GetRequiredService<ITwilioConfig>());

            logger.Verbose($"({label}) Verbose");
            logger.Information($"({label}) Information");
            logger.Debug($"({label}) Debug");
            logger.Warning($"({label}) Warning");
            logger.Error($"({label}) Error");
            logger.Fatal($"({label}) Fatal");

            logger.Elements(EntryElements.All).Information($"({label}) Fully annotated");

            Console.WriteLine("\n\n");
        }
    }
}
