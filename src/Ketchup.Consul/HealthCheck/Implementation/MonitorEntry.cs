using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Ketchup.Consul.HealthCheck.Implementation
{
    public class MonitorEntry
    {
        /// <summary>
        /// 不健康次数
        /// </summary>
        public int UnhealthyTimes { get; set; }

        public EndPoint EndPoint { get; set; }

        /// <summary>
        /// 是否健康
        /// </summary>
        public bool IsHealth { get; set; } = true;
    }
}
