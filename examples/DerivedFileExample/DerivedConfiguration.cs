using System.Collections.Generic;
using J4JSoftware.Logging;
#pragma warning disable 8618

namespace J4JLogger.Examples
{
    public class DerivedConfiguration : J4JLoggerConfiguration
    {
        public class SomeOther
        {
            public int Property1 { get; set; }
            public string Property2 { get; set; }
        }

        public bool SomeOtherProperty { get; set; }
        public List<string> SomeOtherArray { get; set; }
        public SomeOther SomeOtherObject { get; set; }
    }
}
