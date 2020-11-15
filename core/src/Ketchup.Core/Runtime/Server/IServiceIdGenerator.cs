using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Ketchup.Core.Runtime.Server
{
    public interface IServiceIdGenerator
    {
        /// <summary>
        /// 生成一个服务Id。
        /// </summary>
        /// <param name="method">本地方法信息。</param>
        /// <returns>对应方法的唯一服务Id。</returns>
        string GenerateServiceId(MethodInfo method);
    }
}
