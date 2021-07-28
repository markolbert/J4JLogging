using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J4JSoftware.Logging
{
    public class TwilioChannelInfo : ChannelInfo
    {
        public string AccountSID { get; set; } = string.Empty;
        public string AccountToken { get; set; } = string.Empty;
        public string FromNumber { get; set; } = string.Empty;
        public List<string> Recipients { get; set; } = new();
    }
}
