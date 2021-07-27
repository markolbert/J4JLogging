using System;
using System.Linq;

namespace J4JSoftware.Logging
{
    public static class J4JChannelExtensions
    {
        public static ChannelConfigNG<> IncludeSourcePath( this ChannelConfigNG<> channelConfig )
        {
            channelConfig.IncludeSourcePath= true;

            return channelConfig;
        }

        public static ChannelConfigNG<> ExcludeSourcePath(this ChannelConfigNG<> channelConfig)
        {
            channelConfig.IncludeSourcePath = false;

            return channelConfig;
        }

        public static ChannelConfigNG<> SetSourceRootPath(this ChannelConfigNG<> channelConfig, string path)
        {
            channelConfig.SourceRootPath = path;
            channelConfig.IncludeSourcePath= true;

            return channelConfig;
        }

        public static ChannelConfigNG<> ClearSourceRootPath( this ChannelConfigNG<> channelConfig )
        {
            channelConfig.SourceRootPath= null;

            return channelConfig;
        }

        public static ChannelConfigNG<> OutputMultiLineEvents( this ChannelConfigNG<> channelConfig )
        {
            channelConfig.MultiLineEvents = true;

            return channelConfig;
        }

        public static ChannelConfigNG<> OutputSingleLineEvents( this ChannelConfigNG<> channelConfig )
        {
            channelConfig.MultiLineEvents = false;

            return channelConfig;
        }

        public static ChannelConfigNG<> SetOutputTemplate( this ChannelConfigNG<> channelConfig, string template )
        {
            channelConfig.OutputTemplate = template;
            channelConfig.Logger.ResetBaseLogger();

            return channelConfig;
        }

        public static ChannelConfigNG<> ResetOutputTemplate( this ChannelConfigNG<> channelConfig )
        {
            channelConfig.OutputTemplate = J4JLogger.DefaultOutputTemplate;
            channelConfig.Logger.ResetBaseLogger();

            return channelConfig;
        }

        public static ChannelConfigNG<> UseNewLineInOutput( this ChannelConfigNG<> channelConfig )
        {
            channelConfig.RequireNewline = true;
            channelConfig.Logger.ResetBaseLogger();

            return channelConfig;
        }

        public static ChannelConfigNG<> ClearNewLineInOutput( this ChannelConfigNG<> channelConfig )
        {
            channelConfig.RequireNewline = false;
            channelConfig.Logger.ResetBaseLogger();

            return channelConfig;
        }
    }
}