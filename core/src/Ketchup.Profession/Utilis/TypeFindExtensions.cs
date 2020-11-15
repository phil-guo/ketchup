using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ketchup.Core;

namespace Ketchup.Profession.Utilis
{
    public static class TypeFindExtensions
    {
        private static readonly object SyncObj = new object();
        private static Type[] _types;

        public static Type[] Find(Func<Type, bool> predicate)
        {
            return GetAllTypes().Where(predicate).ToArray();
        }

        private static Type[] GetAllTypes()
        {
            if (_types == null)
            {
                lock (SyncObj)
                {
                    if (_types == null)
                    {
                        _types = ContainerBuilderExtensions.GetTypes();
                    }
                }
            }

            return _types;
        }
    }
}
