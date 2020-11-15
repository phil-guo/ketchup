namespace Ketchup.Profession.AutoMapper.ObjectMapper
{
    public interface IObjectMapper
    {
        /// <summary>
        /// 将对象转换为另一个对象。 创建一个新对象
        /// </summary>
        /// <typeparam name="TDestination">目标对象的类型</typeparam>
        /// <param name="source">源对象</param>
        TDestination Map<TDestination>(object source);

        /// <summary>
        /// 执行从源对象到现有目标对象的映射
        /// </summary>
        /// <typeparam name="TSource">源对象</typeparam>
        /// <typeparam name="TDestination">目标对象</typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}
