using System.IO;

namespace J4JSoftware.Logging
{
    public interface IJ4JSmsLoggerConfiguration : IJ4JLoggerConfiguration
    {
        StringWriter SmsWriter { get; }
    }
}