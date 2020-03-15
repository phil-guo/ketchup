using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.Route.Implementation.Args
{
    public class ServiceRouteChangedEventArgs : ServiceRouteEventArgs
    {
        /// <summary>
        /// 旧的服务路由信息。
        /// </summary>
        public ServiceRoute OldRoute { get; set; }
    }
}
