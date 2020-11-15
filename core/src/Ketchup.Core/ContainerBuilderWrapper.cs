using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Ketchup.Core
{
    public class ContainerBuilderWrapper
    {
        public ContainerBuilder ContainerBuilder { get; private set; }

        public ContainerBuilderWrapper(ContainerBuilder builder)
        {
            ContainerBuilder = builder;
        }

        public IContainer Build()
        {
            return ContainerBuilder.Build();
        }
    }
}
