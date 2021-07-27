using System;
using System.Linq;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class ParameterExtensions
    {
        public static Channel<TParameters> IncludeSourcePath<TParameters>( this Channel<TParameters> channel )
            where TParameters : ChannelParameters
        {
            channel.Parameters ??=
                (TParameters) Activator.CreateInstance( typeof(TParameters), new object[] { channel.Logger } )!;

            channel.Parameters = channel.Parameters with { IncludeSourcePath = true };

            return channel;
        }

        public static Channel<TParameters> ExcludeSourcePath<TParameters>( this Channel<TParameters> channel)
            where TParameters : ChannelParameters
        {
            channel.Parameters ??=
                (TParameters)Activator.CreateInstance(typeof(TParameters), new object[] { channel.Logger })!;

            channel.Parameters = channel.Parameters with { IncludeSourcePath = false };
            return channel;
        }

        public static Channel<TParameters> SetSourceRootPath<TParameters>( this Channel<TParameters> channel, string path)
            where TParameters : ChannelParameters
        {
            channel.Parameters ??=
                (TParameters)Activator.CreateInstance(typeof(TParameters), new object[] { channel.Logger })!;

            channel.Parameters = channel.Parameters with { IncludeSourcePath = true, SourceRootPath = path };
            return channel;
        }

        public static Channel<TParameters> ClearSourceRootPath<TParameters>( this Channel<TParameters> channel )
            where TParameters : ChannelParameters
        {
            channel.Parameters ??=
                (TParameters)Activator.CreateInstance(typeof(TParameters), new object[] { channel.Logger })!;

            channel.Parameters = channel.Parameters with { SourceRootPath = null };
            return channel;
        }

        public static Channel<TParameters> OutputMultiLineEvents<TParameters>( this Channel<TParameters> channel )
            where TParameters : ChannelParameters
        {
            channel.Parameters ??=
                (TParameters)Activator.CreateInstance(typeof(TParameters), new object[] { channel.Logger })!;

            channel.Parameters = channel.Parameters with { MultiLineEvents = true };
            return channel;
        }

        public static Channel<TParameters> OutputSingleLineEvents<TParameters>( this Channel<TParameters> channel )
            where TParameters : ChannelParameters
        {
            channel.Parameters ??=
                (TParameters)Activator.CreateInstance(typeof(TParameters), new object[] { channel.Logger })!;

            channel.Parameters = channel.Parameters with { MultiLineEvents = false };
            return channel;
        }

        public static Channel<TParameters> SetOutputTemplate<TParameters>( this Channel<TParameters> channel, string template )
            where TParameters : ChannelParameters
        {
            channel.Parameters ??=
                (TParameters)Activator.CreateInstance(typeof(TParameters), new object[] { channel.Logger })!;

            channel.Parameters = channel.Parameters with { OutputTemplate = template };
            return channel;
        }

        public static Channel<TParameters> ResetOutputTemplate<TParameters>( this Channel<TParameters> channel )
            where TParameters : ChannelParameters
        {
            channel.Parameters ??=
                (TParameters)Activator.CreateInstance(typeof(TParameters), new object[] { channel.Logger })!;

            channel.Parameters = channel.Parameters with { OutputTemplate = J4JBaseLogger.DefaultOutputTemplate };
            return channel;
        }

        public static Channel<TParameters> UseNewLineInOutput<TParameters>( this Channel<TParameters> channel )
            where TParameters : ChannelParameters
        {
            channel.Parameters ??=
                (TParameters)Activator.CreateInstance(typeof(TParameters), new object[] { channel.Logger })!;

            channel.Parameters = channel.Parameters with { RequireNewLine = true };
            return channel;
        }

        public static Channel<TParameters> ClearNewLineInOutput<TParameters>( this Channel<TParameters> channel )
            where TParameters : ChannelParameters
        {
            channel.Parameters ??=
                (TParameters)Activator.CreateInstance(typeof(TParameters), new object[] { channel.Logger })!;

            channel.Parameters = channel.Parameters with { RequireNewLine = false };
            return channel;
        }

        public static Channel<TParameters> MinimumLevel<TParameters>( this Channel<TParameters> channel, LogEventLevel minLevel )
            where TParameters : ChannelParameters
        {
            channel.Parameters ??=
                (TParameters)Activator.CreateInstance(typeof(TParameters), new object[] { channel.Logger })!;

            channel.Parameters = channel.Parameters with { MinimumLevel = minLevel };
            return channel;
        }
    }
}