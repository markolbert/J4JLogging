using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public interface IChannelFactory : ILogChannels
    {
        IConfigurationRoot? ConfigurationRoot { get; }
        string? RootKey { get; }
        LastEventConfig? LastEvent { get; }

        IJ4JLoggerConfiguration? GetLoggerConfiguration<TJ4JLogger>()
            where TJ4JLogger : IJ4JLoggerConfiguration, new();
    }
}