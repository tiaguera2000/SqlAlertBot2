using System;
using System.Collections.Generic;
using System.Text;

namespace AlertsBot.Services.StartVerifyAlerts.Model.RabbitQueue
{
    public class ChannelDetails
    {
        public string connection_name { get; set; }
        public string name { get; set; }
        public string node { get; set; }
        public int number { get; set; }
        public string peer_host { get; set; }
        public int peer_port { get; set; }
        public string user { get; set; }


    }
}
