﻿using System.IO;
using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{
    // needed to keep Json.Net deserializer happy
    public class LogTextConfiguration<TSms> : LogChannelConfiguration, IAfterWritingChannel<TSms>
        where TSms: class
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

        public virtual bool Initialize( TSms config ) => true;

        public void AfterWriting()
        {
            ProcessLogMessage(_writer.ToString());

            _writer.GetStringBuilder().Clear();
        }

        protected virtual bool ProcessLogMessage( string mesg ) => true;

        bool IAfterWritingChannel.Initialize( object config )
        {
            var converted = config as TSms;

            if( converted == null )
                return false;

            return Initialize( converted );
        }
    }
}