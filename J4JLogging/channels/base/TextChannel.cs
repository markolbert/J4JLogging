using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{
    // Base class for IJ4JLogger channels that work by post-processing a Serilog LogEvent
    public class TextChannel : LogChannel, IPostProcess
    {
        private readonly StringWriter _writer = new StringWriter();

        protected TextChannel( J4JLoggerConfiguration config, IJ4JChannelConfig channelConfig )
            :base(config, channelConfig)
        {
        }

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return string.IsNullOrEmpty(OutputTemplate)
                ? sinkConfig.TextWriter(_writer, restrictedToMinimumLevel: MinimumLevel)
                : sinkConfig.TextWriter(_writer, restrictedToMinimumLevel: MinimumLevel, outputTemplate: OutputTemplate);
        }

        public void PostProcess()
        {
            ProcessLogMessage(_writer.ToString());

            Clear();
        }

        public void Clear()
        {
            _writer.GetStringBuilder().Clear();
        }

        protected virtual bool ProcessLogMessage( string mesg ) => true;
    }
}