using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class ActivateStoreRequest
    {
        public Guid AdminId { get; set; }
        public Guid StoreId { get; set; }
        public bool Activate { get; set; }
        public string Reason { get; set; }
    }
}
