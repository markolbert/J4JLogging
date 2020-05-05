using System;
using System.Linq;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace J4JLoggingTests
{
    public class DerivedConfigurationFileTest : ConfigurationFileTests<DerivedConfiguration>
    {
        [Theory]
        [InlineData("Derived.json")]
        public override void Log_event_derived_config_class(string filePath)
        {
            base.Log_event_derived_config_class(filePath);
        }

        protected override void DIResolve(IServiceProvider services)
        {
            base.DIResolve(services);

            var config = services.GetRequiredService<DerivedConfiguration>();

            var twilio = config.Channels.FirstOrDefault(
                    c => c.Channel.Equals("Twilio", StringComparison.OrdinalIgnoreCase))
                as TwilioChannel;

            twilio?.Initialize(services.GetRequiredService<ITwilioConfig>());
        }
    }
}