using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Requests
{
    public class PublishProductReview
    {
        public Guid AdminId { get; set; }
        public Guid ProductReviewId { get; set; }
        public bool ToPublish { get; set; }
        public string Reason { get; set; }
    }
}
