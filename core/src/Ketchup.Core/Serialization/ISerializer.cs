using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.Serialization
{
    public interface ISerializer<T>
    {
        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="instance">需要序列化的对象。</param>
        /// <returns>序列化之后的结果。</returns>
        T Serialize(object instance);

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="content">序列化的内容。</param>
        /// <param name="type">对象类型。</param>
        /// <returns>一个对象实例。</returns>
        object Deserialize(T content, Type type);
    }
}
