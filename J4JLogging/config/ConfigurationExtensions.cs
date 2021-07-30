using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J4JSoftware.Logging
{
    public static class ConfigurationExtensions
    {
        public static J4JBaseLogger ApplyGlobalConfiguration( this J4JBaseLogger logger, IChannelParameters global )
        {
            logger.IncludeSourcePath = global.IncludeSourcePath;
            logger.SourceRootPath = global.SourceRootPath;
            logger.OutputTemplate = global.OutputTemplate;
            logger.RequireNewLine = global.RequireNewLine;
            logger.MinimumLevel = global.MinimumLevel;

            return logger;
        }
    }
}
