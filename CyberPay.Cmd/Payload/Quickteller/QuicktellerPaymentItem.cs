using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public class QuicktellerPaymentItem
    {
        [JsonProperty("categoryid")]
        public string Categoryid { get; set; }
        [JsonProperty("billerid")]
        public string BillerId { get; set; }
        [JsonProperty("isAmountFixed")]
        public bool IsAmountFixed { get; set; }
        [JsonProperty("paymentitemid")]
        public string PaymentItemId { get; set; }
        [JsonProperty("paymentitemname")]
        public string PaymentItemName { get; set; }
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("currencyCode")]
        public string CurrencyCode { get; set; }
        [JsonProperty("currencySymbol")]
        public string CurrencySymbol { get; set; }
        [JsonProperty("itemCurrencySymbol")]
        public string ItemCurrencySymbol { get; set; }
        [JsonProperty("sortOrder")]
        public string SortOrder { get; set; }
        [JsonProperty("pictureId")]
        public string PictureId { get; set; }
        [JsonProperty("paymentCode")]
        public string PaymentCode { get; set; }
    }

    public class QuicktellerPaymentItemReq
    {
        [JsonProperty("TerminalID")]
        public String TerminalID { get; set; }
        [JsonProperty("BillerId")]
        public String BillerId { get; set; }
    }
}
