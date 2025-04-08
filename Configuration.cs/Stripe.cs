using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public static class Stripe
    {
        public static string PaymentUrl = AppSettings.StripePaymentUrl();
        public static string TransKey = AppSettings.StripeKey();
        public static string PublicKey = AppSettings.StripePublicKey();
        public static string WebHookSecret = AppSettings.WebHookSecret();
    }
}
