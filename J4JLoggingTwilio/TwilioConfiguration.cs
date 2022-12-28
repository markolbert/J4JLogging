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

using System.Collections.Generic;
using System.Linq;

namespace J4JSoftware.Logging
{
    public class TwilioConfiguration
    {
        public string? AccountSID { get; set; }
        public string? AccountToken { get; set; }
        public string? FromNumber { get; set; }
        public List<string>? Recipients { get; set; }

        public bool IsValid =>
            !string.IsNullOrEmpty( AccountSID )
            && !string.IsNullOrEmpty( AccountToken )
            && !string.IsNullOrEmpty( FromNumber )
            && ( Recipients?.Any() ?? false );
    }
}
