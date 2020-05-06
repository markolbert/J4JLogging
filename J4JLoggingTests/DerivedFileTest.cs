using Xunit;

namespace J4JLoggingTests
{
    public class DerivedFileTest : DerivedFileTestBase<DerivedConfiguration>
    {
        [ Theory ]
        [ InlineData( "Derived.json" ) ]
        public override void Log_event_derived_config_class( string filePath )
        {
            base.Log_event_derived_config_class( filePath );
        }
    }
}