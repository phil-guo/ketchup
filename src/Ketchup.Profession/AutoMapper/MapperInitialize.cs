using System.Reflection;
using AutoMapper;
using Ketchup.Profession.Utilis;

namespace Ketchup.Profession.AutoMapper
{
    public static class MapperInitialize
    {
        private static readonly object SyncObj = new object();
        private static bool _createdMappingsBefore;

        /// <summary>
        /// 自动创建映射
        /// </summary>
        public static void CreateMappings()
        {
            lock (SyncObj)
            {
                //我们应该防止应用程序中的重复映射，因为Mapper是静态的
                if (_createdMappingsBefore)
                {
                    return;
                }

                Mapper.Initialize(FindAndAutoMapTypes);

                _createdMappingsBefore = true;
            }
        }

        private static void FindAndAutoMapTypes(IMapperConfigurationExpression configuration)
        {
            var types = TypeFindExtensions.Find(type =>
                {
                    var typeInfo = type.GetTypeInfo();
                    return typeInfo.IsDefined(typeof(AutoMapAttribute));
                }
            );

            foreach (var type in types)
            {
                configuration.CreateAutoAttributeMaps(type);
            }
        }
    }
}
