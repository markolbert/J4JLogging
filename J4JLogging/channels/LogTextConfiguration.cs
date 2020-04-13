using System.IO;
using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{
    // needed to keep Json.Net deserializer happy
    public class LogTextConfiguration : LogChannelConfiguration, IAfterWritingChannel
    {
        private readonly StringWriter _writer = new StringWriter();

        public LogTextConfiguration()
            : base( LogChannel.TextWriter )
        {
        }

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.TextWriter( _writer, restrictedToMinimumLevel: MinimumLevel );
        }

        public void AfterWriting()
        {
            ProcessLogMessage(_writer.ToString());

            _writer.GetStringBuilder().Clear();
        }

        protected virtual void ProcessLogMessage( string mesg )
        {
        }
    }

    public class LogTwilioConfiguration : LogTextConfiguration
    {
        protected override void ProcessLogMessage( string mesg )
        {
            base.ProcessLogMessage( mesg );
        }
    }
}