using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Payload.Quickteller
{
    public class BillsPaymentResponseViewModel : QuicktellerBaseServiceResponse
    {
        public String ResponseCode { get; set; }
        public String TransactionRef { get; set; }
        public String ResponseMessage { get; set; }
        public String AmountPaid { get; set; }

        public BillsPaymentResponseViewModel()
        {
        }
    }
}
