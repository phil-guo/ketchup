using System;
using Autofac;
using Ketchup.Core;
using Ketchup.Core.Modules;
using Ketchup.Core.Utilities;
using Microsoft.Extensions.Logging;

namespace Ketchup.Log4net
{
    public class Log4netModule : KernelModule
    {
        private string log4NetConfigFile = "$log4net.config";

        public override void Initialize(KetchupPlatformContainer builder)
        {
            log4NetConfigFile = EnvironmentHelper.GetEnvironmentVariable(log4NetConfigFile);
            builder.GetInstances<ILoggerFactory>().AddProvider(new Log4NetProvider(log4NetConfigFile));
        }
    }
}
