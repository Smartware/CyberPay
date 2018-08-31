using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public class BillPaymentNotification
    {
        [JsonProperty("terminalId")]
        public String TerminalId { get; set; }
        [JsonProperty("paymentCode")]
        public String PaymentCode { get; set; }
        [JsonProperty("customerId")]
        public String CustomerId { get; set; }
        [JsonProperty("customerMobile")]
        public String CustomerMobile { get; set; }
        [JsonProperty("customerEmail")]
        public String CustomerEmail { get; set; }
        [JsonProperty("amount")]
        public String Amount { get; set; }
        [JsonProperty("requestReference")]
        public String RequestReference { get; set; }
    }
}
