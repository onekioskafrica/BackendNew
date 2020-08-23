using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Helpers
{
    public enum OrderStatusEnum
    {
        NEW = 1,
        CHECKOUT = 2,
        PAID = 3,
        COMPLETE = 4,
        ABANDONED = 5
    }
}
