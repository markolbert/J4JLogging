using System.IO;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class J4JSmsLoggerConfiguration : J4JLoggerConfiguration, IJ4JSmsLoggerConfiguration
    {
        public StringWriter SmsWriter { get; } = new StringWriter();
    }
}