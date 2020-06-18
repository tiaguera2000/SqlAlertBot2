using System;
using System.Collections.Generic;
using System.Text;

namespace AlertsBot.Services.StartVerifyAlerts.Model.RabbitQueue
{
    public class MessageStats
    {
        public int ack { get; set; }
        public AckDetails ack_details { get; set; }
        public int deliver { get; set; }
        public DeliverDetails deliver_details { get; set; }
        public int deliver_get { get; set; }
        public DeliverGetDetails deliver_get_details { get; set; }
        public int deliver_no_ack { get; set; }
        public DeliverNoAckDetails deliver_no_ack_details { get; set; }
        public int get { get; set; }
        public GetDetails get_details { get; set; }
        public int get_empty { get; set; }
        public GetEmptyDetails get_empty_details { get; set; }
        public int get_no_ack { get; set; }
        public GetNoAckDetails get_no_ack_details { get; set; }
        public int publish { get; set; }
        public PublishDetails publish_details { get; set; }
        public int redeliver { get; set; }
        public RedeliverDetails redeliver_details { get; set; }
    }
}
