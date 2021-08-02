using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Serilog;
using Serilog.Configuration;
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

        public static TChannel ConfigureChannel<TChannel>( 
            this TChannel channel,
            ChannelConfiguration? configValues = null )
            where TChannel : IChannel, new()
        {
            if (configValues == null)
                return channel;

            if (configValues.RequireNewLine.HasValue)
                channel.RequireNewLine = configValues.RequireNewLine.Value;

            if (configValues.MinimumLevel.HasValue)
                channel.MinimumLevel = configValues.MinimumLevel.Value;

            if (configValues.IncludeSourcePath.HasValue)
                channel.IncludeSourcePath = configValues.IncludeSourcePath.Value;

            if (configValues.OutputTemplate != null)
                channel.OutputTemplate = configValues.OutputTemplate;

            channel.SourceRootPath = configValues.SourceRootPath;

            return channel;
        }

        public static J4JLogger ConfigureLogger(
            this J4JLogger logger,
            ChannelConfiguration? configValues = null)
        {
            if (configValues == null)
                return logger;

            if (configValues.RequireNewLine.HasValue)
                logger.RequireNewLine = configValues.RequireNewLine.Value;

            if (configValues.MinimumLevel.HasValue)
                logger.MinimumLevel = configValues.MinimumLevel.Value;

            if (configValues.IncludeSourcePath.HasValue)
                logger.IncludeSourcePath = configValues.IncludeSourcePath.Value;

            if (configValues.OutputTemplate != null)
                logger.OutputTemplate = configValues.OutputTemplate;

            logger.SourceRootPath = configValues.SourceRootPath;

            return logger;
        }

        private static TChannel AddChannel<TChannel>( this J4JLogger logger, ChannelConfiguration? configValues = null )
            where TChannel : IChannel, new()
        {
            var retVal = new TChannel();
            retVal.SetAssociatedLogger( logger );

            logger.Channels.Add( retVal );

            retVal.ConfigureChannel( configValues );

            return retVal;
        }

        public static DebugChannel AddDebug( this J4JLogger logger, ChannelConfiguration? parameters = null ) =>
            logger.AddChannel<DebugChannel>( parameters );

        public static ConsoleChannel AddConsole( this J4JLogger logger, ChannelConfiguration? parameters = null ) =>
            logger.AddChannel<ConsoleChannel>( parameters );

        public static LastEventChannel AddLastEvent( this J4JLogger logger, ChannelConfiguration? parameters = null ) =>
            logger.AddChannel<LastEventChannel>( parameters );

        public static NetEventChannel AddNetEvent( this J4JLogger logger, ChannelConfiguration? parameters = null ) =>
            logger.AddChannel<NetEventChannel>( parameters );

        public static FileChannel ConfigureFileChannel( 
            this FileChannel channel,
            FileConfiguration? configValues = null )
        {
            if (configValues == null)
                return channel;

            channel = ConfigureChannel( channel, configValues );

            channel.FileName = configValues.FileName;
            channel.Folder = configValues.Folder;
            channel.RollingInterval = configValues.RollingInterval;

            return channel;
        }

        public static FileChannel AddFile( this J4JLogger logger, FileConfiguration? configValues = null )
        {
            configValues ??= new FileConfiguration();

            var retVal = logger.AddChannel<FileChannel>( configValues );

            retVal.ConfigureFileChannel( configValues );

            return retVal;
        }

    }
}