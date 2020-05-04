﻿using System;
using System.Collections.Generic;
using System.Text;
using J4JSoftware.Logging;
using Serilog.Events;

namespace J4JLoggingTest
{
    public class EmbeddedConfiguration
    {
        public class SomeOther
        {
            public int Property1 { get; set; }
            public string Property2 { get; set; }
        }

        public bool SomeOtherProperty { get; set; }
        public List<string> SomeOtherArray { get; set; }
        public SomeOther SomeOtherObject { get; set; }
        public J4JLoggerConfiguration Logging { get; set; }
    }
}
