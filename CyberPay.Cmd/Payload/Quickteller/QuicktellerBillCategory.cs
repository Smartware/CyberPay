using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public class QuicktellerBillCategory
    {
        [JsonProperty("categoryId")]
        public String CategoryId { get; set; }
        [JsonProperty("categoryName")]
        public String CategoryName { get; set; }
        [JsonProperty("categoryDescription")]
        public String CategoryDescription { get; set; }
    }
}
