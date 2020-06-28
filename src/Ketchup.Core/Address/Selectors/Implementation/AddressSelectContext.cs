using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.Address.Selectors.Implementation
{
    public class AddressSelectContext
    {
        public string Name { get; set; }

        /// <summary>
        /// 哈希参数
        /// </summary>
        public string Item { get; set; }
        /// <summary>
        /// 服务可用地址。
        /// </summary>
        public IEnumerable<AddressModel> Address { get; set; }
    }
}
