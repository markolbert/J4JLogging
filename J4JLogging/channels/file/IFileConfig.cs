using Serilog;

namespace J4JSoftware.Logging
{
    public interface IFileConfig : IChannelConfig
    {
        LogFileLocation Location { get; set; }
        RollingInterval RollingInterval { get; set; }
        string FilePath { get; set; }
        string FileName { get; set; }
    }
}