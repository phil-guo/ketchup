using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.Modules
{
    public class KernelModuleProvider : IKernelModuleProvider
    {
        private readonly List<KernelModule> _modules;
        private readonly KetchupPlatformContainer _container;

        public KernelModuleProvider(List<KernelModule> modules, KetchupPlatformContainer container)
        {
            _modules = modules;
            _container = container;
        }

        public List<KernelModule> Modules
        {
            get => _modules;
        }
        public void Initialize()
        {
            _modules.ForEach(item =>
            {
                try
                {
                    item.Initialize(_container);
                }
                catch (Exception e)
                {
                }
            });
        }
    }
}
