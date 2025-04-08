using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.SingletonReferences;
using AdTool.BusinessLogic.DataAccess;
using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.Logging;
using Configuration;
using Google.Apis.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Utils
{
    public class AsinApiUtils
    {
        public async Task<HttpResponseMessage> CallAsinSearchApi(string EndPoint)
        {

            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                string url = AsinApiInfo.AsinApi;

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.Timeout = TimeSpan.FromMinutes(3);
                    var request = new HttpRequestMessage(HttpMethod.Get, EndPoint);

                    response = await client.SendAsync(request);
                }

                if (response == null || !response.IsSuccessStatusCode)
                {
                    string responseMessage = response.ToString();

                    if (string.IsNullOrEmpty(responseMessage))
                    {
                        responseMessage = "response is null or empty";
                    }
                    await ErrorLogging.LogError(responseMessage, "CallAsinSearchApi", EndPoint, null);


                    using (HttpClient client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri(url);
                        client2.Timeout = TimeSpan.FromMinutes(3);
                        var request = new HttpRequestMessage(HttpMethod.Get, EndPoint);

                        response = await client2.SendAsync(request);
                    }
                }
                else
                {
                    return response;
                }
            }
            catch(Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CallAsinSearchApi";
                logError.ClientId = Guid.Empty;
                logError.Parameters = EndPoint;
                await logging.WriteToLog(logError);

                response.StatusCode = HttpStatusCode.InternalServerError;
            }
  

            return response;
        }


    }
}
