using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Ketchup.Core.Address;
using Ketchup.Core.Address.Selectors.Implementation;

namespace Ketchup.Consul.Internal.Selector.Implementation
{
    public class PollingAddressSelector : AddressSelectorBase, IConsulAddressSelector
    {
        private readonly ConcurrentDictionary<string, Lazy<AddressEntry>> concurrent =
            new ConcurrentDictionary<string, Lazy<AddressEntry>>();


        protected override ValueTask<AddressModel> SelectManyAsync(AddressSelectContext context)
        {
            var address = context.Address.ToArray();

            ValidateAddressChange(context.Name, address);

            var addressEntry = concurrent.GetOrAdd(context.Name,
                k => new Lazy<AddressEntry>(() => new AddressEntry(address))).Value;

            var addressModel = addressEntry.GetAddress();

            return new ValueTask<AddressModel>(addressModel);
        }

        private void ValidateAddressChange(string name, AddressModel[] addressModels)
        {
            if (!concurrent.TryGetValue(name, out var value))
                return;

            if (value.Value.AddressModels.Length == addressModels.Length)
                return;

            concurrent.TryRemove(name, out var addressValue);
        }
    }
}