using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public abstract class QuicktellerBaseServiceResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }

        [JsonProperty("errors")]
        public List<ServiceError> Errors { get; set; } = new List<ServiceError>();

        [JsonProperty("error")]
        public ServiceError Error { get; set; } = new ServiceError();

        public bool HasErrors
        {
            get
            {
                return Errors.Any();
            }
        }
    }
}
