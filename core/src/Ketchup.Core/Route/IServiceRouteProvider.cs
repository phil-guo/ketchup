using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ketchup.Core.Route
{
    public interface IServiceRouteProvider
    {
        Task RegisterRoutes();
    }
}
