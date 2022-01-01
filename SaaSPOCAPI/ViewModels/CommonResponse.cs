using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaaSPOCAPI.ViewModels
{
    public class CommonResponse
    {
        public CommonResponseEnum StatusCode { get; set; }
        public string Msg { get; set; }
        public object ResponseObject { get; set; }
    }
    public enum CommonResponseEnum
    {
        Success = 0,
        Failed = 1,
        UnAuthorized = 2,
        Duplicate = 3,
        Updated = 4,
        ResendOTP = 5, 
    }
}
