using System;
using System.Collections.Generic;
using System.Text;

namespace AlertsBot.Services.StartVerifyAlerts.Model.RabbitQueue
{
    public class Queue
    {
        public string name { get; set; }
        public string vhost { get; set; }
    }
}
