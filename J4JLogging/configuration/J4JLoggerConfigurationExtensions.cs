using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Base;

namespace J4JSoftware.Logging.configuration
{
    public static class J4JLoggerConfigurationExtensions
    {
        public static bool CreateConfiguration<TConfig>( string jsonText, out TConfig? result )
            where TConfig : class, IJ4JLoggerConfiguration
        {
            result = null;

            if( string.IsNullOrEmpty( jsonText ) )
                return false;

            try
            {
                result = System.Text.Json.JsonSerializer.Deserialize<TConfig>(jsonText);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public static bool CreateConfigurationFromFile<TConfig>( string jsonPath, out TConfig? result )
            where TConfig : class, IJ4JLoggerConfiguration
        {
            result = null;

            try
            {
                if( !File.Exists( jsonPath ) )
                    return false;

                result = System.Text.Json.JsonSerializer.Deserialize<TConfig>( File.ReadAllText( jsonPath ) );
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
