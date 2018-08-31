using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public class QuicktellerBiller
    {
        [JsonProperty("categoryid")]
        public String CategoryId { get; set; }

        [JsonProperty("categoryname")]
        public String CategoryName { get; set; }

        [JsonProperty("categorydescription")]
        public String CategoryDescription { get; set; }

        [JsonProperty("billerid")]
        public String BillerId { get; set; }

        [JsonProperty("billername")]
        public String BillerName { get; set; }

        [JsonProperty("customerfield1")]
        public String CustomerField1 { get; set; }

        [JsonProperty("customerfield2")]
        public String CustomerField2 { get; set; }

        [JsonProperty("supportemail")]
        public String SupportEmail { get; set; }

        [JsonProperty("paydirectProductId")]
        public String PaydirectProductId { get; set; }

        [JsonProperty("paydirectInstitutionId")]
        public String PaydirectInstitutionId { get; set; }
        [JsonProperty("shortName")]
        public String shortName { get; set; }

        [JsonProperty("narration")]
        public String Narration { get; set; }

        [JsonProperty("surcharge")]
        public String SurCharge { get; set; }
        [JsonProperty("currencyCode")]
        public String CurrencyCode { get; set; }
        [JsonProperty("quickTellerSiteUrlName")]
        public String QuickTellerSiteUrlName { get; set; }

        [JsonProperty("amountType")]
        public String AmountType { get; set; }

        [JsonProperty("currencySymbol")]
        public String CcurrencySymbol { get; set; }

        [JsonProperty("customSectionUrl")]
        public String CustomSectionUrl { get; set; }

        [JsonProperty("logoUrl")]
        public String LogoUrl { get; set; }
        [JsonProperty("type")]
        public String Type { get; set; }
        [JsonProperty("url")]
        public String Url { get; set; }
    }
}
