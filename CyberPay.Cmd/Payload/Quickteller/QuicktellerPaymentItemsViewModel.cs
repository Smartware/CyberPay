using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public class QuicktellerPaymentItemsViewModel: QuicktellerBaseServiceResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        [JsonProperty("paymentitems")]
        public List<QuicktellerPaymentItem> PaymentItems { get; set; }
    }
}
