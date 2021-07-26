using System;
using System.Linq;

namespace J4JSoftware.Logging
{
    public static class J4JLoggerExtensions
    {
        public static J4JLogger SetLoggedType<T>( this J4JLogger logger )=>
            logger.SetLoggedType( typeof( T ) );

        public static J4JLogger SetLoggedType( this J4JLogger logger, Type typeToLog )
        {
            logger.LoggedType = typeToLog;
            logger.ResetBaseLogger();

            return logger;
        }

        public static J4JLogger ClearLoggedType(this J4JLogger logger )
        {
            if (logger.LoggedType == null)
                return logger;

            logger.LoggedType= null;
            logger.ResetBaseLogger();

            return logger;
        }

        public static J4JLogger IncludeSourcePath( this J4JLogger logger )
        {
            logger.IncludeSourcePath= true;

            return logger;
        }

        public static J4JLogger ExcludeSourcePath(this J4JLogger logger)
        {
            logger.IncludeSourcePath = false;

            return logger;
        }

        public static J4JLogger SetSourceRootPath(this J4JLogger logger, string path)
        {
            logger.SourceRootPath = path;
            logger.IncludeSourcePath= true;

            return logger;
        }

        public static J4JLogger ClearSourceRootPath( this J4JLogger logger )
        {
            logger.SourceRootPath= null;

            return logger;
        }

        public static J4JLogger OutputMultiLineEvents( this J4JLogger logger )
        {
            logger.MultiLineEvents = true;

            return logger;
        }

        public static J4JLogger OutputSingleLineEvents( this J4JLogger logger )
        {
            logger.MultiLineEvents = false;

            return logger;
        }

        public static J4JLogger SetOutputTemplate( this J4JLogger logger, string template )
        {
            logger.OutputTemplate = template;
            logger.ResetBaseLogger();

            return logger;
        }

        public static J4JLogger ResetOutputTemplate( this J4JLogger logger )
        {
            logger.OutputTemplate = J4JLogger.DefaultOutputTemplate;
            logger.ResetBaseLogger();

            return logger;
        }

        public static J4JLogger UseNewLineInOutput( this J4JLogger logger )
        {
            logger.RequireNewline = true;
            logger.ResetBaseLogger();

            return logger;
        }

        public static J4JLogger ClearNewLineInOutput( this J4JLogger logger )
        {
            logger.RequireNewline = false;
            logger.ResetBaseLogger();

            return logger;
        }

        public static bool AddChannel<T>( this J4JLogger logger, out T? channelConfig )
            where T : ChannelConfigNG, new()
        {
            channelConfig = null;

            if( logger.ChannelsInternal.Any( x => x.GetType() == typeof( T ) ) )
                return false;

            channelConfig = new T();

            logger.ChannelsInternal.Add( channelConfig );
            logger.ResetBaseLogger();

            return true;
        }

        public static bool RemoveChannel<T>( this J4JLogger logger )
            where T : ChannelConfigNG, new()
        {
            var index = logger.ChannelsInternal
                .FindIndex( x => x is T );

            if( index < 0 )
                return false;

            logger.ChannelsInternal.RemoveAt( index );
            logger.ResetBaseLogger();

            return true;
        }
    }
}