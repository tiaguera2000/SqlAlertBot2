using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertsBot.WebApi.Model
{
    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string state { get; set; }
    }
}
