﻿#region license

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

namespace J4JSoftware.Logging
{
    public class SourceFileEnricher : BaseEnricher
    {
        public SourceFileEnricher()
            : base("SourceFilePath")
        {
        }

        public override bool EnrichContext => !string.IsNullOrEmpty(SourceFilePath);
        public override object GetValue() => SourceFilePath!;

        public string? SourceFilePath { get; set; }
    }
}