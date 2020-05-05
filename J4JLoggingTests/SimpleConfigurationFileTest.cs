using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace J4JLoggingTests
{
    public class SimpleConfigurationFileTest : ConfigurationFileTests<J4JLoggerConfiguration>
    {
        [Theory]
        [InlineData("Simple.json")]
        public override void Log_event_derived_config_class( string filePath )
        {
            base.Log_event_derived_config_class( filePath );
        }

        protected override void DIResolve( IServiceProvider services )
        {
            base.DIResolve( services );

            var config = services.GetRequiredService<J4JLoggerConfiguration>();

            var twilio = config.Channels.FirstOrDefault(
                    c => c.Channel.Equals("Twilio", StringComparison.OrdinalIgnoreCase))
                as TwilioChannel;

            var twilioConfig = services.GetRequiredService<ITwilioConfig>();
            twilioConfig.Should().NotBe( null );

            twilio?.Initialize(services.GetRequiredService<ITwilioConfig>());
        }
    }
}
