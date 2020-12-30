using System;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class J4JCachedLogger : IJ4JLogger
    {
        private J4JCachedLoggerInternal _internal;
        private bool _includeSms;

        public J4JCachedLogger( J4JLoggerCache? cache = null )
        {
            Cache = cache ?? new J4JLoggerCache();

            _internal = new J4JCachedLoggerInternal( Cache );
        }

        public J4JLoggerCache Cache { get; }
        
        public void SetLoggedType<TLogged>()
        {
            _internal = _internal.SetLoggedType( typeof(TLogged) );
        }

        public void SetLoggedType( Type toLog )
        {
            _internal = _internal.SetLoggedType( toLog );
        }

        // do nothing; no reason to output cache when we're caching entries
        public bool OutputCache( J4JLoggerCache cache ) => false;

        public IJ4JLogger IncludeSms()
        {
            _includeSms = true;
            return this;
        }

        public void Write( LogEventLevel level, string template, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            _internal.Add( _includeSms, level, template, memberName, srcPath, srcLine );
            _includeSms = false;
        }

        public void Write<T0>( LogEventLevel level, string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            _internal.Add( _includeSms, level, template, memberName, srcPath, srcLine, propertyValue );
            _includeSms = false;
        }

        public void Write<T0, T1>( LogEventLevel level, string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "", int srcLine = 0 )
        {
            _internal.Add( _includeSms, level, template, memberName, srcPath, srcLine, propertyValue0, propertyValue1 );
            _includeSms = false;
        }

        public void Write<T0, T1, T2>( LogEventLevel level, string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0 )
        {
            _internal.Add( _includeSms, level, template, memberName, srcPath, srcLine, propertyValue0, propertyValue1,
                propertyValue2 );
            _includeSms = false;
        }

        public void Write( LogEventLevel level, string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0 )
        {
            _internal.Add(_includeSms, level, template, memberName, srcPath, srcLine, propertyValues);
            _includeSms = false;
        }

        public void Debug( string template, string memberName = "", string srcPath = "", int srcLine = 0 ) =>
            Write( LogEventLevel.Debug, template, memberName, srcPath, srcLine );

        public void Debug<T0>( string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0 ) =>
            Write( LogEventLevel.Debug, template, propertyValue, memberName, srcPath, srcLine );

        public void Debug<T0, T1>( string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0 ) =>
            Write( LogEventLevel.Debug, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine );

        public void Debug<T0, T1, T2>( string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0 ) =>
            Write( LogEventLevel.Debug, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath,
                srcLine );

        public void Debug( string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0 ) =>
            Write( LogEventLevel.Debug, template, propertyValues, memberName, srcPath,
                srcLine );

        public void Error(string template, string memberName = "", string srcPath = "", int srcLine = 0) =>
            Write(LogEventLevel.Error, template, memberName, srcPath, srcLine);

        public void Error<T0>(string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Error, template, propertyValue, memberName, srcPath, srcLine);

        public void Error<T0, T1>(string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Error, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine);

        public void Error<T0, T1, T2>(string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0) =>
            Write(LogEventLevel.Error, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath,
                srcLine);

        public void Error(string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Error, template, propertyValues, memberName, srcPath,
                srcLine);

        public void Fatal(string template, string memberName = "", string srcPath = "", int srcLine = 0) =>
            Write(LogEventLevel.Fatal, template, memberName, srcPath, srcLine);

        public void Fatal<T0>(string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Fatal, template, propertyValue, memberName, srcPath, srcLine);

        public void Fatal<T0, T1>(string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Fatal, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine);

        public void Fatal<T0, T1, T2>(string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0) =>
            Write(LogEventLevel.Fatal, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath,
                srcLine);

        public void Fatal(string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Fatal, template, propertyValues, memberName, srcPath,
                srcLine);

        public void Information(string template, string memberName = "", string srcPath = "", int srcLine = 0) =>
            Write(LogEventLevel.Information, template, memberName, srcPath, srcLine);

        public void Information<T0>(string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Information, template, propertyValue, memberName, srcPath, srcLine);

        public void Information<T0, T1>(string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Information, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine);

        public void Information<T0, T1, T2>(string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0) =>
            Write(LogEventLevel.Information, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath,
                srcLine);

        public void Information(string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Information, template, propertyValues, memberName, srcPath,
                srcLine);

        public void Verbose(string template, string memberName = "", string srcPath = "", int srcLine = 0) =>
            Write(LogEventLevel.Verbose, template, memberName, srcPath, srcLine);

        public void Verbose<T0>(string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Verbose, template, propertyValue, memberName, srcPath, srcLine);

        public void Verbose<T0, T1>(string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Verbose, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine);

        public void Verbose<T0, T1, T2>(string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0) =>
            Write(LogEventLevel.Verbose, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath,
                srcLine);

        public void Verbose(string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Verbose, template, propertyValues, memberName, srcPath,
                srcLine);

        public void Warning(string template, string memberName = "", string srcPath = "", int srcLine = 0) =>
            Write(LogEventLevel.Warning, template, memberName, srcPath, srcLine);

        public void Warning<T0>(string template, T0 propertyValue, string memberName = "", string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Warning, template, propertyValue, memberName, srcPath, srcLine);

        public void Warning<T0, T1>(string template, T0 propertyValue0, T1 propertyValue1, string memberName = "",
            string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Warning, template, propertyValue0, propertyValue1, memberName, srcPath, srcLine);

        public void Warning<T0, T1, T2>(string template, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            string memberName = "", string srcPath = "", int srcLine = 0) =>
            Write(LogEventLevel.Warning, template, propertyValue0, propertyValue1, propertyValue2, memberName, srcPath,
                srcLine);

        public void Warning(string template, object[] propertyValues, string memberName = "", string srcPath = "",
            int srcLine = 0) =>
            Write(LogEventLevel.Warning, template, propertyValues, memberName, srcPath,
                srcLine);
    }
}