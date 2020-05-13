using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Ketchup.Core.Address;

namespace Ketchup.Consul.Internal.Selector.Implementation
{
    public class AddressEntry
    {
        private int _index;
        private int _lock;
        private readonly int _maxIndex;

        public AddressModel[] AddressModels { get; set; }

        public AddressEntry(AddressModel[] addressModels)
        {
            AddressModels = addressModels;
            _maxIndex = addressModels.Length - 1;
        }

        public AddressModel GetAddress()
        {
            while (true)
            {
                //如果无法得到锁则等待
                if (Interlocked.Exchange(ref _lock, 1) != 0)
                {
                    default(SpinWait).SpinOnce();
                    continue;
                }
                var _address = AddressModels[_index];

                //设置为下一个
                if (_maxIndex > _index)
                    _index++;
                else
                    _index = 0;

                //释放锁
                Interlocked.Exchange(ref _lock, 0);

                return _address;
            }
        }

    }
}
