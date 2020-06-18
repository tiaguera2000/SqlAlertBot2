using System;
using System.Collections.Generic;
using System.Text;

namespace AlertsBot.Services.StartVerifyAlerts.Model.RabbitQueue
{
    public class GarbageCollection
    {
        public int fullsweep_after { get; set; }
        public int max_heap_size { get; set; }
        public int min_bin_vheap_size { get; set; }
        public int min_heap_size { get; set; }
        public int minor_gcs { get; set; }
    }
}
