using System;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class J4JLoggerExtensions
    {
        public static J4JBaseLogger SetLoggedType<TLogged>( this J4JBaseLogger logger ) =>
            logger.SetLoggedType( typeof(TLogged) );

        public static J4JBaseLogger SetLoggedType( this J4JBaseLogger logger, Type typeToLog )
        {
            logger.LoggedType = typeToLog;
            return logger;
        }

        public static J4JBaseLogger ClearLoggedType(this J4JBaseLogger logger )
        {
            logger.LoggedType= null;
            return logger;
        }

        public static J4JBaseLogger IncludeSourcePath(this J4JBaseLogger logger)
        {
            logger.Parameters = logger.Parameters with { IncludeSourcePath = true };
            return logger;
        }

        public static J4JBaseLogger ExcludeSourcePath(this J4JBaseLogger logger)
        {
            logger.Parameters = logger.Parameters with { IncludeSourcePath = false };
            return logger;
        }

        public static J4JBaseLogger SetSourceRootPath(this J4JBaseLogger logger, string path)
        {
            logger.Parameters = logger.Parameters with { IncludeSourcePath = true, SourceRootPath = path };
            return logger;
        }

        public static J4JBaseLogger ClearSourceRootPath(this J4JBaseLogger logger)
        {
            logger.Parameters = logger.Parameters with { SourceRootPath = null };
            return logger;
        }

        public static J4JBaseLogger OutputMultiLineEvents(this J4JBaseLogger logger)
        {
            logger.Parameters = logger.Parameters with { MultiLineEvents = true };
            return logger;
        }

        public static J4JBaseLogger OutputSingleLineEvents(this J4JBaseLogger logger)
        {
            logger.Parameters = logger.Parameters with { MultiLineEvents = false };
            return logger;
        }

        public static J4JBaseLogger SetOutputTemplate(this J4JBaseLogger logger, string template)
        {
            logger.Parameters = logger.Parameters with { OutputTemplate = template };
            return logger;
        }

        public static J4JBaseLogger ResetOutputTemplate(this J4JBaseLogger logger)
        {
            logger.Parameters = logger.Parameters with { OutputTemplate = J4JBaseLogger.DefaultOutputTemplate };
            return logger;
        }

        public static J4JBaseLogger UseNewLineInOutput(this J4JBaseLogger logger)
        {
            logger.Parameters = logger.Parameters with { RequireNewLine = true };
            return logger;
        }

        public static J4JBaseLogger ClearNewLineInOutput(this J4JBaseLogger logger)
        {
            logger.Parameters = logger.Parameters with { RequireNewLine = false };
            return logger;
        }

        public static J4JBaseLogger MinimumLevel(this J4JBaseLogger logger, LogEventLevel minLevel)
        {
            logger.Parameters = logger.Parameters with { MinimumLevel = minLevel };
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

            retVal.Parameters = retVal.Parameters == null
                ? new ChannelParameters( null ) { MinimumLevel = minLevel }
                : retVal.Parameters with { MinimumLevel = minLevel };

            logger.Channels.Add( retVal );

            return retVal;
        }

        public static ConsoleChannel AddConsole(this J4JLogger logger, LogEventLevel minLevel = LogEventLevel.Verbose)
        {
            var retVal = new ConsoleChannel(logger);

            retVal.Parameters = retVal.Parameters == null
                ? new ChannelParameters(null) { MinimumLevel = minLevel, RequireNewLine = true }
                : retVal.Parameters with { MinimumLevel = minLevel, RequireNewLine = true };

            logger.Channels.Add(retVal);

            return retVal;
        }

        public static FileConfig AddFile(
            this J4JLogger logger,
            LogEventLevel minLevel = LogEventLevel.Verbose,
            string fileStub = "log.txt",
            RollingInterval interval = RollingInterval.Day,
            string? folder = null )
        {
            var retVal = new FileConfig( logger );

            retVal.Parameters = retVal.Parameters == null
                ? new FileParameters( logger )
                {
                    MinimumLevel = minLevel,
                    FileName = fileStub,
                    RollingInterval = interval,
                    Folder = folder ?? Environment.CurrentDirectory
                }
                : retVal.Parameters! with
                {
                    MinimumLevel = minLevel,
                    FileName = fileStub,
                    RollingInterval = interval,
                    Folder = folder ?? Environment.CurrentDirectory
                };

            logger.Channels.Add( retVal );

            return retVal;
        }

        public static LastEventChannel AddLastEvent(
            this J4JLogger logger,
            LogEventLevel minLevel = LogEventLevel.Verbose )
        {
            var retVal = new LastEventChannel(logger);

            retVal.Parameters = retVal.Parameters == null
                ? new ChannelParameters(null) { MinimumLevel = minLevel }
                : retVal.Parameters with { MinimumLevel = minLevel };


            logger.Channels.Add(retVal);

            return retVal;
        }

        public static NetEventChannel AddNetEvent(
            this J4JLogger logger,
            LogEventLevel minLevel = LogEventLevel.Verbose)
        {
            var retVal = new NetEventChannel(logger);

            retVal.Parameters = retVal.Parameters == null
                ? new ChannelParameters(null) { MinimumLevel = minLevel }
                : retVal.Parameters with { MinimumLevel = minLevel };

            logger.Channels.Add(retVal);

            return retVal;
        }
    }
}