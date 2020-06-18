using System;
using System.Collections.Generic;
using System.Text;

namespace AlertsBot.Services.StartVerifyAlerts.Model.RabbitQueue
{
    public class RabbitQueue
    {
        public List<ConsumerDetail> consumer_details { get; set; }
        public Arguments2 arguments { get; set; }
        public bool auto_delete { get; set; }
        public BackingQueueStatus backing_queue_status { get; set; }
        public double? consumer_utilisation { get; set; }
        public int consumers { get; set; }
        public List<object> deliveries { get; set; }
        public bool durable { get; set; }
        public EffectivePolicyDefinition effective_policy_definition { get; set; }
        public bool exclusive { get; set; }
        public object exclusive_consumer_tag { get; set; }
        public GarbageCollection garbage_collection { get; set; }
        public object head_message_timestamp { get; set; }
        public List<object> incoming { get; set; }
        public int memory { get; set; }
        public MessageStats message_stats { get; set; }
        public int messages { get; set; }
        public MessagesDetails messages_details { get; set; }
        public int messages_paged_out { get; set; }
        public int messages_persistent { get; set; }
        public int messages_ram { get; set; }
        public int messages_ready { get; set; }
        public MessagesReadyDetails messages_ready_details { get; set; }
        public int messages_ready_ram { get; set; }
        public int messages_unacknowledged { get; set; }
        public MessagesUnacknowledgedDetails messages_unacknowledged_details { get; set; }
        public int messages_unacknowledged_ram { get; set; }
        public string name { get; set; }
        public string node { get; set; }
        public object operator_policy { get; set; }
        public object policy { get; set; }
        public object recoverable_slaves { get; set; }
        public long reductions { get; set; }
        public ReductionsDetails reductions_details { get; set; }
        public object single_active_consumer_tag { get; set; }
        public string state { get; set; }
        public string type { get; set; }
        public string vhost { get; set; }
    }
}
