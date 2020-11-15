using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Core.Address;
using Ketchup.Core.Address.Selectors.Implementation;

namespace Ketchup.Consul.Internal.Selector.Implementation
{
    public class RandomWeightAddressSelector : AddressSelectorBase, IConsulAddressSelector
    {
        private readonly ConcurrentDictionary<string, AddressEntry> concurrent =
            new ConcurrentDictionary<string, AddressEntry>();

        private readonly Func<int, int, int> _generate;
        private readonly Random _random;

        public RandomWeightAddressSelector()
        {
            _random = new Random();
            _generate = (min, max) => _random.Next(min, max);
        }

        protected override ValueTask<AddressModel> SelectManyAsync(AddressSelectContext context)
        {
            var address = context.Address.ToList();

            foreach (var addressModel in address)
            {
                var model = addressModel as IpAddressModel;
                var weight = Convert.ToInt32(model?.Meta[SelectorType.RandomWeight.ToString()]);
                for (int i = 0; i < weight; i++)
                {
                    address.Add(model);
                }
            }

            var length = address.Count;

            var index = _generate(0, length);
            return new ValueTask<AddressModel>(address[index]);
        }
    }
}
