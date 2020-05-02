using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{
    public class TextChannel<TSms> : LogChannel, IPostProcess<TSms>
        where TSms: class
    {
        private readonly StringWriter _writer = new StringWriter();

        protected TextChannel()
        {
        }

        protected TextChannel(IConfigurationRoot configRoot, string loggerSection = "Logger")
            : base(configRoot, loggerSection)
        {
        }

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.TextWriter( _writer, restrictedToMinimumLevel: MinimumLevel );
        }

        public virtual bool Initialize( TSms config ) => true;

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

        bool IPostProcess.Initialize( object config )
        {
            var converted = config as TSms;

            if( converted == null )
                return false;

            return Initialize( converted );
        }
    }
}