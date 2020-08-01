using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Contracts.V1.Responses
{
    public class GenericResponse
    {
        public string ResultCode { get; set; }
        public bool Status { get; set; } = false;
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
