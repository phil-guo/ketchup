using AutoMapper;

namespace Ketchup.Profession.AutoMapper
{
    public static class AutoMapExtensions
    {
        /// <summary>
        /// 使用AutoMapper库将对象转换为另一个对象。 是一个新的实例对象。
        /// 在调用此方法之前，必须存在对象之间的映射。
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination MapTo<TDestination>(this object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// 执行从源对象到现有目标对象的映射
        /// 在调用此方法之前，必须存在对象之间的映射。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }
    }
}
