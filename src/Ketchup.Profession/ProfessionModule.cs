using System;
using Autofac;
using Ketchup.Core;
using Ketchup.Core.Modules;
using Ketchup.Profession.ORM.EntityFramworkCore;
using Ketchup.Profession.ORM.EntityFramworkCore.Repository;
using Ketchup.Profession.ORM.EntityFramworkCore.Repository.Implementation;
using Ketchup.Profession.ORM.EntityFramworkCore.UntiOfWork;
using Ketchup.Profession.ORM.EntityFramworkCore.UntiOfWork.Implementation;
using Ketchup.Profession.Repository;

namespace Ketchup.Profession
{
    public class ProfessionModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
            base.Initialize(builder);
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterGeneric(typeof(EfCoreRepository<,>)).As(typeof(IEfCoreRepository<,>)).InstancePerLifetimeScope();
            builder.ContainerBuilder.RegisterGeneric(typeof(EfUnitOfWork<>)).As(typeof(IEfUnitOfWork));
        }
    }
}
