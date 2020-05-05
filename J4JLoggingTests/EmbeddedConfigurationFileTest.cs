using System;
using System.Linq;
using Autofac;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace J4JLoggingTests
{
    public class EmbeddedConfigurationFileTest : ConfigurationFileTests<EmbeddedConfiguration>
    {
        [Theory]
        [InlineData("Embedded.json")]
        public override void Log_event_derived_config_class(string filePath)
        {
            base.Log_event_derived_config_class(filePath);
        }

        protected override void DIRegister( ContainerBuilder containerBuilder )
        {
            base.DIRegister( containerBuilder );

            containerBuilder.Register(c =>
                {
                    var config = c.Resolve<EmbeddedConfiguration>();

                    return config.Logging;
                })
                .As<IJ4JLoggerConfiguration>();
        }

        protected override void DIResolve(IServiceProvider services)
        {
            base.DIResolve(services);

            var config = services.GetRequiredService<EmbeddedConfiguration>();

            var twilio = config.Logging.Channels.FirstOrDefault(
                    c => c.Channel.Equals("Twilio", StringComparison.OrdinalIgnoreCase))
                as TwilioChannel;

            twilio?.Initialize(services.GetRequiredService<ITwilioConfig>());
        }
    }
}