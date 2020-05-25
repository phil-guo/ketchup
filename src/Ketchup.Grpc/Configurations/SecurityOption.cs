using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Grpc.Configurations
{
    public class SecurityOption
    {
        /// <summary>
        /// 白名单
        /// </summary>
        public string Whitelist { get; set; } = "*";

        /// <summary>
        /// 黑名单
        /// </summary>
        public string BlackList { get; set; }
    }
}
