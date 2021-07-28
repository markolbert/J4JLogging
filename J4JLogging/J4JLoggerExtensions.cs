using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class J4JLoggerExtensions
    {
        public static Func<TProp> GetGlobalAccessor<TProp>(
            this J4JBaseLogger logger,
            Expression<Func<J4JBaseLogger, TProp>> expr)
        {
            var compiled = expr.Compile();

            return () => compiled(logger);
        }

        public static J4JBaseLogger ApplySettings( this J4JBaseLogger container, LoggerInfo loggerInfo )
        {
            if( loggerInfo.Global == null )
                return container;

            container.SourcePathIncluded = loggerInfo.Global.IncludeSourcePath;
            container.MinimumLevel = loggerInfo.Global.MinimumLevel;
            container.OutputTemplate = loggerInfo.Global.OutputTemplate;
            container.RequireNewLine = loggerInfo.Global.RequireNewLine;
            container.SourceRootPath = loggerInfo.Global.SourceRootPath;

            return container;
        }

        public static J4JBaseLogger IncludeSourcePath(this J4JBaseLogger container )
        {
            container.SourcePathIncluded = true;
            return container;
        }

        public static J4JBaseLogger ExcludeSourcePath(this J4JBaseLogger container)
        {
            container.SourcePathIncluded = false;
            return container;
        }

        public static J4JBaseLogger SetSourceRootPath( this J4JBaseLogger container, string path)
        {
            container.SourcePathIncluded = true;
            container.SourceRootPath = path;

            return container;
        }

        public static J4JBaseLogger ClearSourceRootPath( this J4JBaseLogger container)
        {
            container.SourceRootPath = null;
            return container;
        }

        public static J4JBaseLogger SetOutputTemplate( this J4JBaseLogger container, string template)
        {
            container.OutputTemplate = template;
            return container;
        }

        public static J4JBaseLogger ResetOutputTemplate( this J4JBaseLogger container)
        {
            container.OutputTemplate = J4JBaseLogger.DefaultOutputTemplate;
            return container;
        }

        public static J4JBaseLogger UseNewLineInOutput( this J4JBaseLogger container)
        {
            container.RequireNewLine = true;
            return container;
        }

        public static J4JBaseLogger ClearNewLineInOutput( this J4JBaseLogger container)
        {
            container.RequireNewLine = false;
            return container;
        }

        public static J4JBaseLogger MinimumLevel( this J4JBaseLogger container, LogEventLevel minLevel)
        {
            container.MinimumLevel = minLevel;
            return container;
        }

        public static J4JBaseLogger OutputNextEventToSms( this J4JBaseLogger logger )
        {
            logger.OutputNextToSms = true;
            return logger;
        }

        public static IChannel AddChannel<T>( this J4JLogger logger )
            where T : class, IChannel
        {
            var channelType = typeof(T);

            var retVal = logger.Channels.FirstOrDefault( x => x.GetType() == channelType );
            if( retVal != null )
                return retVal;

            try
            {
                retVal = (IChannel) Activator.CreateInstance( channelType, new object[] { logger }, null )!;
            }
            catch( Exception e )
            {
                throw new ArgumentException(
                    $"Could not create instance of '{typeof(T)}'. It must have a public constructor taking an instance of {nameof(J4JBaseLogger)} as its only argument",
                    e );
            }

            logger.Channels.Add( retVal );
            logger.ResetBaseLogger();

            return retVal;
        }

        public static J4JLogger AddChannels(this J4JLogger logger, params IChannel[] channels )
        {
            foreach( var channel in channels.Where( x => 
                logger.Channels.All( y => x.GetType() != y.GetType() ) ) 
            )
            {
                logger.Channels.Add(channel!);
            }

            logger.ResetBaseLogger();

            return logger;
        }

        public static J4JLogger RemoveChannel<T>(this J4JLogger logger)
            where T : class, IChannel, new()
        {
            var index = logger.Channels
                .FindIndex(x => x is T);

            if (index < 0)
                return logger;

            logger.Channels.RemoveAt(index);
            logger.ResetBaseLogger();

            return logger;
        }

        public static DebugChannel AddDebug( this J4JLogger logger, LogEventLevel minLevel = LogEventLevel.Verbose )
        {
            var retVal = new DebugChannel( logger );
            retVal.Parameters.MinimumLevel= minLevel;

            logger.Channels.Add( retVal );

            return retVal;
        }

        public static ConsoleChannel AddConsole(this J4JLogger logger, LogEventLevel minLevel = LogEventLevel.Verbose)
        {
            var retVal = new ConsoleChannel(logger);
            retVal.Parameters.MinimumLevel = minLevel;
            retVal.Parameters.RequireNewLine = true;

            logger.Channels.Add(retVal);

            return retVal;
        }

        public static FileChannel AddFile(
            this J4JLogger logger,
            LogEventLevel minLevel = LogEventLevel.Verbose,
            string fileStub = "log.txt",
            RollingInterval interval = RollingInterval.Day,
            string? folder = null )
        {
            var retVal = new FileChannel( logger );

            retVal.Parameters.MinimumLevel = minLevel;
            retVal.Parameters.FileName = fileStub;
            retVal.Parameters.RollingInterval = interval;
            retVal.Parameters.Folder = folder ?? Environment.CurrentDirectory;

            logger.Channels.Add( retVal );

            return retVal;
        }

        public static LastEventChannel AddLastEvent(
            this J4JLogger logger,
            LogEventLevel minLevel = LogEventLevel.Verbose )
        {
            var retVal = new LastEventChannel(logger);
            retVal.Parameters.MinimumLevel = minLevel;

            logger.Channels.Add(retVal);

            return retVal;
        }

        public static NetEventChannel AddNetEvent(
            this J4JLogger logger,
            LogEventLevel minLevel = LogEventLevel.Verbose)
        {
            var retVal = new NetEventChannel(logger);
            retVal.Parameters.MinimumLevel = minLevel;

            logger.Channels.Add(retVal);

            return retVal;
        }
    }
}