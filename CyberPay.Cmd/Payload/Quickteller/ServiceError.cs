using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public class ServiceError
    {
        [JsonProperty("code")]
        public String Code { get; set; }
        [JsonProperty("message")]
        public String Message { get; set; }
    }
}
