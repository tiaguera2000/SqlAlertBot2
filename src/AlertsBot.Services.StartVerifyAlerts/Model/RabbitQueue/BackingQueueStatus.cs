using System;
using System.Collections.Generic;
using System.Text;

namespace AlertsBot.Services.StartVerifyAlerts.Model.RabbitQueue
{
    public class BackingQueueStatus
    {
        public double avg_ack_egress_rate { get; set; }
        public double avg_ack_ingress_rate { get; set; }
        public double avg_egress_rate { get; set; }
        public double avg_ingress_rate { get; set; }
        public List<object> delta { get; set; }
        public int len { get; set; }
        public string mode { get; set; }
        public int next_seq_id { get; set; }
        public int q1 { get; set; }
        public int q2 { get; set; }
        public int q3 { get; set; }
        public int q4 { get; set; }
        public string target_ram_count { get; set; }
    }
}
