using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.PaymentProcessor.BusinessObjects
{
    public class MerchantAuthentication
    {
        public string name { get; set; } = AppSettings.PaymentLogin();
        public string transactionKey { get; set; } = AppSettings.TransKey();
    }
}
