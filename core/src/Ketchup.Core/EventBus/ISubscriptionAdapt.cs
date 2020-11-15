using System;
using System.Collections.Generic;
using System.Text;

namespace Ketchup.Core.EventBus
{
    public interface ISubscriptionAdapt
    {
        void SubscribeAt();

        void Unsubscribe();
    }
}
