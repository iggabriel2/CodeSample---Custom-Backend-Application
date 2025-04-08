using AdTool.BusinessLogic.Utilities;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Utils
{
    public class ExchnageRateApiUtils
    {
        public async Task<HttpResponseMessage> CallExchangeRateApi()
        {

            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                string url = ExchangeInfo.ExchangeAPI;
                string exchangeKey = ExchangeInfo.ExchangeKey;

                string endpoint = "/v1/latest?access_key=" + exchangeKey + "&base=USD&symbols=GBP,AUD,CAD";

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.Timeout = TimeSpan.FromMinutes(3);
                    var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

                    response = await client.SendAsync(request);
                }

                if (response == null || !response.IsSuccessStatusCode)
                {
                    string responseMessage = response.ToString();

                    if (string.IsNullOrEmpty(responseMessage))
                    {
                        responseMessage = "response is null or empty";
                    }
                    await ErrorLogging.LogError(responseMessage, "CallExchangeRateApi", endpoint, null);


                    using (HttpClient client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri(url);
                        client2.Timeout = TimeSpan.FromMinutes(3);
                        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

                        response = await client2.SendAsync(request);
                    }
                }
                else
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "CallExchangeRateApi", "NA");
                response.StatusCode = HttpStatusCode.InternalServerError;
            }


            return response;
        }
    }
}
