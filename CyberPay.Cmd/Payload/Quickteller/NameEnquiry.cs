using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public class NameEnquiry : QuicktellerBaseServiceResponse
    {
        [JsonProperty("accountName")]
        public string AccountName { get; set; }
    }
}
