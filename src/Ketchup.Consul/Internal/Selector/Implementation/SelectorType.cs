using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Consul.Internal.Selector.Implementation
{
    /// <summary>
    /// 负载算法类型
    /// </summary>
    public enum SelectorType
    {
        /// <summary>
        /// 随机
        /// </summary>
        Random,

        /// <summary>
        /// 轮询
        /// </summary>
        Polling,

        /// <summary>
        /// 随机权重
        /// </summary>
        RandomWeight
    }
}
