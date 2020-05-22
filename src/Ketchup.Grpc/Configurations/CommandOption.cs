using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Grpc.Configurations
{
    public class CommandOption
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        public int TimeOut { get; set; } = 10000;

        /// <summary>
        /// 白名单
        /// </summary>
        public string Whitelist { get; set; } = "*";

        /// <summary>
        /// 黑名单
        /// </summary>
        public string BlackList { get; set; }

        /// <summary>
        /// 限流数
        /// </summary>
        public int MaxBulkhead { get; set; }
    }
}
