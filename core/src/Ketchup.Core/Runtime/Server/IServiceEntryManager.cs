using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.Runtime.Server
{
    /// <summary>
    /// 一个抽象的服务条目管理者
    /// </summary>
    public interface IServiceEntryManager
    {
        /// <summary>
        /// 获取服务条目集合。
        /// </summary>
        /// <returns>服务条目集合。</returns>
        IEnumerable<ServiceEntry> GetEntries();

        void UpdateEntries(IEnumerable<IServiceEntryProvider> providers);
    }
}
