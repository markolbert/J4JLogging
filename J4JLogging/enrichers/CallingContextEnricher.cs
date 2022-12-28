// Copyright (c) 2021, 2022 Mark A. Olbert 
// 
// This file is part of J4JLogger.
//
// J4JLogger is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// J4JLogger is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with J4JLogger. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Text;

namespace J4JSoftware.Logging;

public class CallingContextEnricher : J4JEnricher
{
    public static string DefaultFilePathTrimmer( Type? loggedType,
        string callerName,
        int lineNum,
        string srcFilePath )
    {
        var sb = new StringBuilder();

        if( loggedType != null )
            sb.Append( loggedType.FullName );

        sb.Append( $"::{callerName} ({srcFilePath}:{lineNum})" );

        return sb.ToString();
    }

    public static string RemoveProjectPath(
        string rawPath,
        string projPath,
        StringComparison textComparison =
            StringComparison.OrdinalIgnoreCase
    ) =>
        !string.IsNullOrEmpty( projPath ) && rawPath.StartsWith( projPath, textComparison )
            ? rawPath.Replace( projPath, string.Empty )
            : rawPath;

    public CallingContextEnricher()
        : base( "CallingContext" )
    {
    }

    public Func<Type?, string, int, string, string> FilePathTrimmer { get; set; } = DefaultFilePathTrimmer;

    public override bool EnrichContext =>
        !string.IsNullOrEmpty( CallingMemberName )
     && !string.IsNullOrEmpty( SourceFilePath )
     && LineNumber > 0;

    public override object GetValue() =>
        FilePathTrimmer( LoggedType, CallingMemberName!, LineNumber, SourceFilePath! );

    public string? CallingMemberName { get; set; }
    public Type? LoggedType { get; set; }
    public int LineNumber { get; set; }
    public string? SourceFilePath { get; set; }
}