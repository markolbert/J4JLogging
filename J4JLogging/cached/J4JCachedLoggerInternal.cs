using System;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class J4JCachedLoggerInternal
    {
        private readonly J4JLoggerCache _cache;

        internal J4JCachedLoggerInternal( J4JLoggerCache cache )
        {
            _cache = cache;
        }

        private J4JCachedLoggerInternal( J4JCachedLoggerInternal toCopy )
        {
            _cache = toCopy._cache;
        }

        public J4JCachedLoggerInternal SetLoggedType( Type loggedType ) =>
            new J4JCachedLoggerInternal( this ) { LoggedType = loggedType };

        public Type? LoggedType { get; private set; }

        public void Add(
            bool includeSms,
            LogEventLevel level,
            string template,
            string memberName,
            string sourcePath,
            int sourceLine,
            params object[] propertyValues ) 
            => _cache.Add( LoggedType, includeSms, level, template, memberName,
            sourcePath, sourceLine, propertyValues );
    }
}