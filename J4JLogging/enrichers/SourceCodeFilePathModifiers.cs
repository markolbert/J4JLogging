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

using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace J4JSoftware.Logging
{
    public static class SourceCodeFilePathModifiers
    {
        // copy these next two methods to the source code file where you configure J4JLogger
        // and then reference FilePathTrimmer as the context converter you
        // want to use
        private static string FilePathTrimmer( Type? loggedType,
                                               string callerName,
                                               int lineNum,
                                               string srcFilePath )
        {
            return CallingContextEnricher.DefaultFilePathTrimmer( loggedType,
                                                                 callerName,
                                                                 lineNum,
                                                                 CallingContextEnricher.RemoveProjectPath( srcFilePath,
                                                                  GetProjectPath() ) );
        }

        private static string GetProjectPath( [ CallerFilePath ] string filePath = "" )
        {
            // DirectoryInfo will throw an exception when this method is called on a machine
            // other than the development machine, so just return an empty string in that case
            try
            {
                var dirInfo = new DirectoryInfo(System.IO.Path.GetDirectoryName(filePath)!);

                while (dirInfo.Parent != null)
                {
                    if (dirInfo.EnumerateFiles("*.csproj").Any())
                        break;

                    dirInfo = dirInfo.Parent;
                }

                return dirInfo.FullName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
