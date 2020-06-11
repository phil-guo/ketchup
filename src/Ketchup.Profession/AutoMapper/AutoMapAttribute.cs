using System;
using AutoMapper;

namespace Ketchup.Profession.AutoMapper
{
    /// <summary>
    /// AutoMap 属性映射
    /// </summary>
    public class AutoMapAttribute : AutoMapAttributeBase
    {
        public AutoMapAttribute(params Type[] targetTypes)
            : base(targetTypes)
        {

        }
        public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
        {
            if (TargetTypes == null || TargetTypes.Length <= 0)
            {
                return;
            }

            foreach (var targetType in TargetTypes)
            {
                configuration.CreateMap(type, targetType, MemberList.Source);
                configuration.CreateMap(targetType, type, MemberList.Destination);
            }
        }
    }
}
