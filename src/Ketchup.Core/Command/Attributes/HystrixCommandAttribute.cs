using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.Command.Attributes
{
    public class HystrixCommandAttribute : Attribute
    {
        public string MethodName { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; } = 10000;

        /// <summary>
        /// 白名单
        /// </summary>
        public string Whitelist { get; set; } = "*";

        /// <summary>
        /// 黑名单
        /// </summary>
        public string BlackList { get; set; }

        /// <summary>
        /// 最大信号量
        /// </summary>
        public int MaxRequests { get; set; } = 0;

        /// <summary>
        /// 最大信号量的限定时间
        /// 默认1s
        /// </summary>
        public int MaxRequestsTime { get; set; } = 1000;
    }
}
