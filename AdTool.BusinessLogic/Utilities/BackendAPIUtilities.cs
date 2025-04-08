using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.Logging;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.Utilities
{
    public class BackendAPIUtilities
    {
        public async Task<HttpResponseMessage> CallPostApi(string serlializedJson, string endpoint, Guid clientId)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(General.BackendApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var stringContent = new StringContent(serlializedJson, UnicodeEncoding.UTF8, "application/json");

                response = await client.PostAsync(endpoint, stringContent);
            }

            if (response.IsSuccessStatusCode)
            {
                //handle any special api items here, like invalid token
            }
            else
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = response.Content.ToString();
                logError.FailureMethod = "CallPostApi";
                logError.ClientId = clientId;
                logError.Parameters = serlializedJson;
                await logging.WriteToLog(logError);

            }

            return response;

        }
    }
}
