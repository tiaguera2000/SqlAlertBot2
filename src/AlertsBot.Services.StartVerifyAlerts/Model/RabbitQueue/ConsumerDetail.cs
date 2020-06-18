using System;
using System.Collections.Generic;
using System.Text;

namespace AlertsBot.Services.StartVerifyAlerts.Model.RabbitQueue
{
    public class ConsumerDetail
    {
        public ChannelDetails channel_details { get; set; }
        public bool ack_required { get; set; }
        public bool active { get; set; }
        public string activity_status { get; set; }
        public string consumer_tag { get; set; }
        public bool exclusive { get; set; }
        public int prefetch_count { get; set; }
        public Queue queue { get; set; }
    }
}
