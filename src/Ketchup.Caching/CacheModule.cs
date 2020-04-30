using Autofac;
using Ketchup.Caching.Configurations;
using Ketchup.Caching.Internal;
using Ketchup.Caching.Internal.Memory;
using Ketchup.Caching.Internal.Redis;
using Ketchup.Core;
using Ketchup.Core.Modules;

namespace Ketchup.Caching
{
    public class CacheModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
            base.Initialize(builder);
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            var appConfig = new AppConfig();

            builder.ContainerBuilder.RegisterType<MemoryCacheProvider>()
                .Named<ICacheProvider>(CacheModel.Memory.ToString());
            builder.ContainerBuilder.RegisterType<RedisCacheProvider>()
                .Named<ICacheProvider>(CacheModel.Redis.ToString())
                .WithParameter(new TypedParameter(typeof(CacheOption), appConfig.Cache));
        }
    }
}