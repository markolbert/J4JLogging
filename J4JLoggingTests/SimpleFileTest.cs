using J4JSoftware.Logging;
using Xunit;

namespace J4JLoggingTests
{
    public class SimpleFileTest : DerivedFileTestBase<J4JLoggerConfiguration>
    {
        [Theory]
        [InlineData("Simple.json")]
        public override void Log_event_derived_config_class( string filePath )
        {
            base.Log_event_derived_config_class( filePath );
        }
    }
}
