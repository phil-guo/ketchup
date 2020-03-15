using System;
using System.Linq;
using System.Threading.Tasks;
using Ketchup.Core.Address;
using Ketchup.Core.Address.Selectors.Implementation;

namespace Ketchup.Consul.Selector.Implementation
{
    public class ConsulRandomAddressSelector : AddressSelectorBase,IConsulAddressSelector
    {
        private readonly Func<int, int, int> _generate;
        private readonly Random _random;

        public ConsulRandomAddressSelector()
        {
            _random = new Random();
            _generate = (min, max) => _random.Next(min, max);
        }

        protected override ValueTask<AddressModel> SelectManyAsync(AddressSelectContext context)
        {
            var address = context.Address.ToArray();
            var length = address.Length;

            var index = _generate(0, length);
            return new ValueTask<AddressModel>(address[index]);
        }
    }
}
