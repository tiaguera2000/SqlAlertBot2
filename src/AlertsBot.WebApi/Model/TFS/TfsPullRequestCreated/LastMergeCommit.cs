﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertsBot.WebApi.Model
{
    public class LastMergeCommit
    {
        public string commitId { get; set; }
        public string url { get; set; }
    }
}
