using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ketchup.Core.Address.Selectors.Implementation
{
    public abstract class AddressSelectorBase : IAddressSelector
    {
        public async ValueTask<AddressModel> SelectAsync(AddressSelectContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Descriptor == null)
                throw new ArgumentNullException(nameof(context.Descriptor));
            if (context.Address == null)
                throw new ArgumentNullException(nameof(context.Address));

            if (context.Address.Count() == 0)
                throw new ArgumentException("没有任何地址信息。", nameof(context.Address));

            if (context.Address.Count() == 1)
            {
                return context.Address.First();
            }
            else
            {
                var vt = SelectManyAsync(context);
                return vt.IsCompletedSuccessfully ? vt.Result : await vt;
            }
        }

        protected abstract ValueTask<AddressModel> SelectManyAsync(AddressSelectContext context);
    }
}
