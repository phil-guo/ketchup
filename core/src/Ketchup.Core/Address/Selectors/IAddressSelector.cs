using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ketchup.Core.Address.Selectors.Implementation;

namespace Ketchup.Core.Address.Selectors
{
    public interface IAddressSelector
    {
        ValueTask<AddressModel> SelectAsync(AddressSelectContext context);
    }
}
