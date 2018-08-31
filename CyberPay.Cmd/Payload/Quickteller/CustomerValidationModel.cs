using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public class CustomerValidationModel
    {

        [JsonProperty("customers")]
        public List<QuicktellerCustomerViewModel> customers { get; set; }

    }
}
