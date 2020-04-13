using System;
using Serilog;

namespace J4JSoftware.Logging
{
    public class J4JLoggerFactory : IJ4JLoggerFactory
    {
        private readonly ILogger _baseLogger;
        private readonly IJ4JLoggerConfiguration _config;

        public J4JLoggerFactory(
            ILogger baseLogger,
            IJ4JLoggerConfiguration config )
        {
            _baseLogger = baseLogger ?? throw new NullReferenceException( nameof(baseLogger) );
            _config = config ?? throw new NullReferenceException( nameof(config) );
        }

        public IJ4JLogger CreateLogger( Type toLog )
        {
            if( toLog == null )
                throw new NullReferenceException( nameof(toLog) );

            return new J4JLogger( toLog, _baseLogger, _config );
        }
    }
}