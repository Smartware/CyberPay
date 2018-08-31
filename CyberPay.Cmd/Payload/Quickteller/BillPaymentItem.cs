using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public class BillPaymentItem
    {

        [JsonProperty("categoryid")]
        public String CategoryId { get; set; }

        [JsonProperty("billerid")]
        public String BillerId { get; set; }

        [JsonProperty("isAmountFixed")]
        public String IsAmountFixed { get; set; }
        [JsonProperty("paymentitemid")]
        public String PaymentItemId { get; set; }

        [JsonProperty("paymentitemname")]
        public String PaymentItemName { get; set; }
        [JsonProperty("amount")]
        public String IswAmount { get; set; }


        public decimal Amount
        {
            get

            {
                return Convert.ToDecimal(IswAmount == "" ? "0" : IswAmount) / 100;

            }
            protected set { }
        }


        [JsonProperty("code")]
        public String Code { get; set; }

        [JsonProperty("currencyCode")]
        public String CurrencyCode { get; set; }

        [JsonProperty("currencySymbol")]
        public String CurrencySymbol { get; set; }

        [JsonProperty("itemCurrencySymbol")]
        public String ItemCurrencySymbol { get; set; }

        [JsonProperty("sortOrder")]
        public String SortOrder { get; set; }

        [JsonProperty("pictureId")]
        public String PictureId { get; set; }

        [JsonProperty("paymentCode")]
        public String PaymentCode { get; set; }

        [JsonProperty("itemFee")]
        public String IswItemFee { get; set; }

        public decimal ItemFee
        {
            get

            {
                return Convert.ToDecimal(IswItemFee == "" ? "0" : IswItemFee) / 100;

            }
            protected set { }

        }
    }
}
