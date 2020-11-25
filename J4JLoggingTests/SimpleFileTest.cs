using System;
using System.IO;
using J4JSoftware.Logging;
using Xunit;

namespace J4JLoggingTests
{
    public class SimpleFileTest
    {
        [Theory]
        [InlineData("config-files/Simple.json", "Logger", "Channels")]
        [InlineData("config-files/Derived.json", "Logger", "Channels")]
        [InlineData("config-files/Embedded.json", "Container:Logger", "Container:Channels")]
        public void Simple( string configPath, string loggerKey, string channelsKey )
        {
            var compRoot = new CompositionRoot( configPath, loggerKey, channelsKey );

            var logger = compRoot.J4JLogger;
            logger.SetLoggedType(this.GetType());

            var console = new StringWriter();
            Console.SetOut( console );

            logger.Verbose<string, string>("{0} ({1})", "Verbose", configPath);
            logger.Warning<string, string>("{0} ({1})", "Warning", configPath);
            logger.Information<string, string>("{0} ({1})", "Information", configPath);
            logger.Debug<string, string>("{0} ({1})", "Debug", configPath);
            logger.Error<string, string>("{0} ({1})", "Error", configPath);
            logger.Fatal<string, string>("{0} ({1})", "Fatal", configPath);

            logger.ForceExternal().Verbose<string, string>( "{0} ({1})", "Verbose", configPath );

            console.Flush();

            var output = console.ToString();
        }
    }
}
