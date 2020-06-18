using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertsBot.WebApi.Model
{
    public class Message
    {
        public string text { get; set; }
        public string html { get; set; }
        public string markdown { get; set; }
    }
}
