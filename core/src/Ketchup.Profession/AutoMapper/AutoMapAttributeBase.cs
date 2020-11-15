using System;
using AutoMapper;

namespace Ketchup.Profession.AutoMapper
{
    public abstract class AutoMapAttributeBase : Attribute
    {
        public Type[] TargetTypes { get; private set; }

        protected AutoMapAttributeBase(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }

        /// <summary>
        /// 创建映射
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="type"></param>
        public abstract void CreateMap(IMapperConfigurationExpression configuration, Type type);
    }
}
