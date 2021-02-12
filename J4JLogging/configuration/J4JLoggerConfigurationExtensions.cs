#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'J4JLogging' is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
// 
// This library or program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with
// this library or program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

using System.IO;
using System.Text.Json;

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
                result = JsonSerializer.Deserialize<TConfig>( jsonText );
            }
            catch
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

                result = JsonSerializer.Deserialize<TConfig>( File.ReadAllText( jsonPath ) );
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}