using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Configuration;

namespace AdTool.PaymentProcessor.Utils
{
    public static class WebhookTokenValidation
    {
        public static async Task<bool> CheckTokens(string data, string AnetSignature1)
        {
            string signatureKey = AppSettings.WebHookKey();
            string AnetSignature = AnetSignature1.Substring(AnetSignature1.IndexOf("=") + 1);
            if (String.IsNullOrEmpty(data)) return false;
            if (String.IsNullOrEmpty(AnetSignature)) return false;
            if (String.IsNullOrEmpty(signatureKey)) return false;

            // generate the shaw token
            var token = await GetSHAToken(data, signatureKey);
            if (String.IsNullOrEmpty(token)) return false;

            return token.Equals(AnetSignature, StringComparison.InvariantCultureIgnoreCase);

        }

        private static async Task<string> GetSHAToken(string data, string key)
        {
            // use Encoding.ASCII.GetBytes or Encoding.UTF8.GetBytes

            byte[] _key = Encoding.ASCII.GetBytes(key);
            using (var myhmacsha1 = new HMACSHA1(_key))
            {
                var hashArray = new HMACSHA512(_key).ComputeHash(Encoding.ASCII.GetBytes(data));

                return hashArray.Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
            }

        }
    }
}
