using CyberPay.Cmd.Payload.Quickteller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPay.Cmd.Providers
{
    public interface IQuickTellerBillProvider
    {
        NameEnquiry ValidateName(string bankCode, string accountId);
        List<QuicktellerBillCategory> GetBillCategories();
        List<QuicktellerBiller> GetBillers();
        BillsPaymentResponseViewModel SendBillPaymentNotification(string paymentcode, string customerUniqueReference, string customerMobile, string customerEmail, string transactionUniqueReference, decimal amount);
        QuicktellerCustomerViewModel ValidateCustomer(string paymentCode, string subscriberId);

        QuicktellerPaymentItemsViewModel GetBillerById(string billerId);
    }
}
