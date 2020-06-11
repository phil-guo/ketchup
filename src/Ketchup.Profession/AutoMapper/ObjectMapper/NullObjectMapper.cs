using System;

namespace Ketchup.Profession.AutoMapper.ObjectMapper
{
    public sealed class NullObjectMapper : IObjectMapper
    {
        /// <summary>
        /// 单利
        /// </summary>
        public static NullObjectMapper Instance { get; } = new NullObjectMapper();

        public TDestination Map<TDestination>(object source)
        {
            throw new NotImplementedException();
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            throw new NotImplementedException();
        }
    }
}
