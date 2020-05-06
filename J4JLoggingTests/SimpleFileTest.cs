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
