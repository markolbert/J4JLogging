using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class ChannelParameterExtensions
    {
        public static ChannelParameters IncludeSourcePath(this ChannelParameters container )
        {
            container.IncludeSourcePath = true;
            return container;
        }

        public static ChannelParameters ExcludeSourcePath(this ChannelParameters container)
        {
            container.IncludeSourcePath = false;
            return container;
        }

        public static ChannelParameters SetSourceRootPath( this ChannelParameters container, string path)
        {
            container.IncludeSourcePath = true;
            container.SourceRootPath = path;

            return container;
        }

        public static ChannelParameters ClearSourceRootPath( this ChannelParameters container)
        {
            container.SourceRootPath = null;
            return container;
        }

        public static ChannelParameters SetOutputTemplate( this ChannelParameters container, string template)
        {
            container.OutputTemplate = template;
            return container;
        }

        public static ChannelParameters ResetOutputTemplate( this ChannelParameters container)
        {
            container.OutputTemplate = J4JBaseLogger.DefaultOutputTemplate;
            return container;
        }

        public static ChannelParameters UseNewLineInOutput( this ChannelParameters container)
        {
            container.RequireNewLine = true;
            return container;
        }

        public static ChannelParameters ClearNewLineInOutput( this ChannelParameters container)
        {
            container.RequireNewLine = false;
            return container;
        }

        public static ChannelParameters MinimumLevel( this ChannelParameters container, LogEventLevel minLevel)
        {
            container.MinimumLevel = minLevel;
            return container;
        }
    }
}