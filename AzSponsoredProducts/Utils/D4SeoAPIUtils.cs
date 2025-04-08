using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.SingletonReferences;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.AzSp.ClientAuthorization;
using AdTool.Entities.AzSp.General;
using AdTool.Entities.Logging;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace AdTool.AzSponsoredProducts.Utils
{
    public class D4SeoAPIUtils
    {
        //this is only calling the US for now
        public async Task<HttpResponseMessage> CallD4PostApi(string EndPoint, string serlializedJson)
        {

            HttpResponseMessage taskPostResponse = new HttpResponseMessage();

            try
            {
                string url = D4ConfigInfo.D4Api;

                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(url),
                    DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(D4ConfigInfo.D4Login))) }
                };

                taskPostResponse = await httpClient.PostAsync(EndPoint, new StringContent(serlializedJson));

                if (taskPostResponse == null || !taskPostResponse.IsSuccessStatusCode)
                {
                    var httpClient2 = new HttpClient
                    {
                        BaseAddress = new Uri(url),
                        DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(D4ConfigInfo.D4Login))) }
                    };

                    taskPostResponse = await httpClient2.PostAsync(EndPoint, new StringContent(serlializedJson));
                }
            }
            catch(Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CallD4PostApi";
                logError.ClientId = Guid.Empty;
                logError.Parameters = JsonSerializer.Serialize(serlializedJson) + EndPoint;
                await logging.WriteToLog(logError);

                taskPostResponse.StatusCode = HttpStatusCode.InternalServerError;
            }

            return taskPostResponse;
        }
    }
}
