using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Ketchup.Core.Exceptions;

namespace Ketchup.Core.Modules
{
    public abstract class KernelModule : Module
    {
        public ContainerBuilderWrapper Builder { get; set; }


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

        protected virtual void RegisterModule(ContainerBuilderWrapper builder)
        {
        }
    }
}
