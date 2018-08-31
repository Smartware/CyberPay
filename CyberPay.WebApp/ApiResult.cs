using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CyberPay.WebApp
{
    public class ApiResult<T>
    {
        public int Code { get; set; }
        public string ResponseMessage { get; set; }
        public T Data { get; set; } = Activator.CreateInstance<T>();
    }
}