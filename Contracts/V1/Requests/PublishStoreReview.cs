using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class PublishStoreReview
    {
        public Guid AdminId { get; set; }
        public Guid StoreReviewId { get; set; }
        public bool ToPublish { get; set; }
        public string Reason { get; set; }
    }
}
