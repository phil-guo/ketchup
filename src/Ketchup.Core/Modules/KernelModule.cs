using System;
using Autofac;
using Ketchup.Core.Exceptions;
using Microsoft.AspNetCore.Routing;

namespace Ketchup.Core.Modules
{
    public abstract class KernelModule : Module
    {


        public ContainerBuilderWrapper Builder { get; set; }

        public virtual void Initialize(KetchupPlatformContainer builder)
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                base.Load(builder);
                Builder = new ContainerBuilderWrapper(builder);
                RegisterModule(Builder);
            }
            catch (Exception exception)
            {
                throw new KetchupPlatformException($"模块注册发生错误{exception.Message}");
            }
        }

        protected virtual void RegisterModule(ContainerBuilderWrapper builder) { }


        public virtual void MapGrpcService(IEndpointRouteBuilder endpointRoute) { }
    }
}
