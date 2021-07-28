using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class ChannelInfo
    {
        public bool IncludeSourcePath { get; set; }
        public string? SourceRootPath { get; set; }
        public string OutputTemplate { get; set; } = J4JBaseLogger.DefaultOutputTemplate;
        public bool RequireNewLine { get; set; }
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Verbose;
    }
}
