using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.Runtime.Server.Attributes
{
    /// <summary>
    /// 服务集标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceBundleAttribute : Attribute
    {
        public ServiceBundleAttribute() { }

        public ServiceBundleAttribute(string routeTemplate)
        {
            RouteTemplate = routeTemplate;
        }
        public string RouteTemplate { get; }
    }
}
