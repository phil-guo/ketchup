using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Ketchup.Core
{
    public class KetchupPlatformContainer
    {
        private IComponentContext _container;

        public IComponentContext Current
        {
            get
            {
                return _container;
            }
            internal set
            {
                _container = value;
            }
        }

        public KetchupPlatformContainer(IComponentContext container)
        {
            this._container = container;
        }

        public bool IsRegistered<T>()
        {
            return _container.IsRegistered<T>();
        }

        public bool IsRegisteredWithKey(string serviceKey, Type serviceType)
        {
            if (!string.IsNullOrEmpty(serviceKey))
                return _container.IsRegisteredWithKey(serviceKey, serviceType);
            else
                return _container.IsRegistered(serviceType);
        }

        public bool IsRegistered<T>(object serviceKey)
        {
            return _container.IsRegisteredWithKey<T>(serviceKey);
        }

        public T GetInstances<T>(string name) where T : class
        {

            return _container.ResolveKeyed<T>(name);
        }

        public T GetInstances<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        public object GetInstances(Type type)
        {
            return _container.Resolve(type);
        }

        public T GetInstances<T>(Type type) where T : class
        {
            return _container.Resolve(type) as T;
        }

        public object GetInstancePerLifetimeScope(string name, Type type)
        {
            return string.IsNullOrEmpty(name) ? GetInstances(type) : _container.ResolveKeyed(name, type);
        }

    }
}
