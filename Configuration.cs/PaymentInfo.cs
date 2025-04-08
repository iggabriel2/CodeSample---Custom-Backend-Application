using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public static class PaymentInfo
    {
        //public static string PaymentApi = "https://apitest.authorize.net/xml/v1/request.api";
        public static string PaymentLogin = AppSettings.PaymentLogin();
        public static string TransKey = AppSettings.TransKey();
        public static string Crypty = AppSettings.Crypty();
    }
}
