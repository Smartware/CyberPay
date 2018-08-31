using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public class QuicktellerServiceJSONResponse : QuicktellerBaseServiceResponse
    {
        [JsonProperty("categorys")]
        public List<QuicktellerBillCategory> categories { get; set; }
        [JsonProperty("billers")]
        public List<QuicktellerBiller> Billers { get; set; }
        [JsonProperty("paymentitems")]
        public List<BillPaymentItem> paymentsitems { get; set; }
        [JsonProperty("customers")]
        public List<QuicktellerCustomerViewModel> customers { get; set; }
        [JsonProperty("accountName")]
        public NameEnquiry accountName { get; set; }
    }
}
