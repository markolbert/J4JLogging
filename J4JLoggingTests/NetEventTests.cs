using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using J4JSoftware.Logging;
using Serilog.Events;
using Xunit;

namespace J4JLoggingTests
{
    public class NetEventTests
    {
        private string _curTemplate = string.Empty;
        private LogEventLevel _curLevel = LogEventLevel.Verbose;
        private IJ4JLogger? _logger;

        [Theory]
        [InlineData(LogEventLevel.Information)]
        [InlineData(LogEventLevel.Error)]
        [InlineData(LogEventLevel.Debug)]
        [InlineData(LogEventLevel.Fatal)]
        [InlineData(LogEventLevel.Warning)]
        [InlineData(LogEventLevel.Verbose)]
        public void TestEvent(LogEventLevel level)
        {
            var loggerConfig = new J4JLoggerConfiguration();

            var netEventConfig = new NetEventConfig
            {
                EventElements = EventElements.None,
                OutputTemplate = "[{Level:u3}] {Message:lj}"
            };

            netEventConfig.LogEvent += NetEventConfigOnLogEvent;

            loggerConfig.Channels.Add( netEventConfig );

            var builder = new ContainerBuilder();
            builder.RegisterJ4JLogging( loggerConfig );

            var container = builder.Build();
            _logger = container.Resolve<IJ4JLogger>();

            LogMessage( level );
        }

        private void LogMessage( LogEventLevel level )
        {
            _curLevel = level;

            var abbr = level switch
            {
                LogEventLevel.Debug => "DBG",
                LogEventLevel.Error => "ERR",
                LogEventLevel.Fatal => "FTL",
                LogEventLevel.Information => "INF",
                LogEventLevel.Verbose => "VRB",
                LogEventLevel.Warning => "WRN",
                _ => throw new InvalidEnumArgumentException( $"Unsupported {nameof(LogEventLevel)} '{level}'" )
            };

            _curTemplate = $"[{abbr}] This is a(n) \"{level}\" event\r\n";

            _logger!.Write( level, "This is a(n) {0} event", level );
        }

        private void NetEventConfigOnLogEvent( object? sender, NetEventArgs e )
        {
            e.Level.Should().Be( _curLevel );
            e.LogMessage.Should().Be( _curTemplate );
        }
    }
}
