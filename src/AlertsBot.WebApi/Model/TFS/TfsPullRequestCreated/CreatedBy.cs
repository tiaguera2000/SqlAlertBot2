using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertsBot.WebApi.Model
{
    public class CreatedBy
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
    }
}
