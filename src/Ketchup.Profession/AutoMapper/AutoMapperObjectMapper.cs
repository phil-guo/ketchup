using AutoMapper;
using IObjectMapper = Ketchup.Profession.AutoMapper.ObjectMapper.IObjectMapper;

namespace Ketchup.Profession.AutoMapper
{
    public class AutoMapperObjectMapper : ObjectMapper.IObjectMapper
    {
        public TDestination Map<TDestination>(object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }
    }
}
