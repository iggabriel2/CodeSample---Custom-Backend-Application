using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.ProductManagement;
using Azure;
using System.Text;
using System;
using System.Text.Json;
using AdTool.Entities.AzSp.ClientAuthorization;
using Configuration;
using System.Runtime.InteropServices;
using AdTool.AzSponsoredProducts.SingletonReferences;
using AdTool.Entities.AzSp.General;
using System.Net.Http.Headers;
using System.Net;
using static Google.Rpc.Context.AttributeContext.Types;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.Logging;
using AdTool.AzSponsoredProducts.Data;
using AdTool.AzSponsoredProducts.AmazonAPI.Tokens;
using System.Net.NetworkInformation;
using Google.Ads.GoogleAds.V11.Common;
using AdTool.BusinessLogic.Utilities;

namespace AdTool.AzSponsoredProducts.Utils
{
    public class AzAPIUtils
    {

        private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(0);
        private static bool LockAmazon = false;

        public async Task<HttpResponseMessage> CallAmazonPostApi(string EndPoint, string MediaType, APIAuthorization auth, ClientProfileCodes profileCode, string serlializedJson)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            if (LockAmazon)
            {
                try
                {
                    await _semaphoreSlim.WaitAsync();
                    response = await CallAmazonPostApiRequest(EndPoint, MediaType, auth, profileCode, serlializedJson);
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
            else
            {
                response = await CallAmazonPostApiRequest(EndPoint, MediaType, auth, profileCode, serlializedJson);
                if (!LockAmazon)
                {
                    _semaphoreSlim.Release();
                }
            }
            return response;
        }

        public async Task<HttpResponseMessage> CallAmazonPutApi(string EndPoint, string MediaType, APIAuthorization auth, ClientProfileCodes profileCode, string serlializedJson)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            if (LockAmazon)
            {
                try
                {
                    await _semaphoreSlim.WaitAsync();
                    response = await CallAmazonPutApiRequest(EndPoint, MediaType, auth, profileCode, serlializedJson);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
            else
            {
                response = await CallAmazonPutApiRequest(EndPoint, MediaType, auth, profileCode, serlializedJson);
                if (!LockAmazon)
                {
                    _semaphoreSlim.Release();
                }
            }
            return response;
        }

        public async Task<HttpResponseMessage> CallAmazonGetApi(string EndPoint, string MediaType, APIAuthorization auth, ClientProfileCodes profileCode, string serlializedJson = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();

            if (LockAmazon)
            {
                try
                {
                    await _semaphoreSlim.WaitAsync();
                    response = await CallAmazonGetApiRequest(EndPoint, MediaType, auth, profileCode, serlializedJson);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _semaphoreSlim.Release();
                }

            }
            else
            {
                response = await CallAmazonGetApiRequest(EndPoint, MediaType, auth, profileCode, serlializedJson);

                if (!LockAmazon)
                {
                    _semaphoreSlim.Release();
                }
            }

            return response;
        }

        public async Task<HttpResponseMessage> CallAmazonGetApiReports(string EndPoint, string MediaType, APIAuthorization auth, ClientProfileCodes profileCode, string serlializedJson = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                await _semaphoreSlim.WaitAsync();
                await System.Threading.Tasks.Task.Delay(1000);
                response = await CallAmazonGetApiRequest(EndPoint, MediaType, auth, profileCode, serlializedJson);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _semaphoreSlim.Release();
            }

            return response;

        }

        public async Task<HttpResponseMessage> CallAmazonApiProfilesOnly(string EndPoint, string MediaType, APIAuthorization auth, int ApiToCall)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            if (LockAmazon)
            {
                try
                {
                    await _semaphoreSlim.WaitAsync();
                    response = await CallAmazonApiProfilesOnlyRequest(EndPoint, MediaType, auth, ApiToCall);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
            else
            {
                response = await CallAmazonApiProfilesOnlyRequest(EndPoint, MediaType, auth, ApiToCall);
                if (!LockAmazon)
                {
                    _semaphoreSlim.Release();
                }
            }
            return response;

        }

        public async Task<HttpResponseMessage> CallAmazonPostApiRequest(string EndPoint, string MediaType, APIAuthorization auth, ClientProfileCodes profileCode, string serlializedJson)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                //get country api
                List<AzApiCountries> countriesList = CountriesSingleton.CountriesList();
                AzApiCountries thisCountry = countriesList.Where(x => x.Id == profileCode.CountryId).FirstOrDefault();

                int apiInt = thisCountry.AzApi;

                string url = "";

                switch (apiInt)
                {
                    case 1:
                        url = AzApiInfo.API1;
                        break;
                    case 2:
                        url = AzApiInfo.API2;
                        break;
                    case 3:
                        url = AzApiInfo.API3;
                        break;

                }

                try
                {
                    if (LockAmazon)
                    { await _semaphoreSlim.WaitAsync(); }

                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                        client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                        client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                        var request = new HttpRequestMessage(HttpMethod.Post, EndPoint);
                        await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Post 203", serlializedJson);

                        request.Content = new StringContent(serlializedJson);
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);

                        response = await client.SendAsync(request);
                    }
                }
                catch(Exception ex)
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                }
                 
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    try
                    {
                        LockAmazon = true;

                        //get value from header and try again
                        IEnumerable<string>? timesToWait = new List<string>();
                        var foundTimeToWait = response.Headers.TryGetValues("Retry-After", out timesToWait);

                        int seconds = 2000;

                        if (foundTimeToWait)
                        {
                            if (timesToWait != null && !string.IsNullOrEmpty(timesToWait.First()))
                            {
                                seconds = Convert.ToInt32(timesToWait.First()) * 1000;
                            }
                        }

                        await System.Threading.Tasks.Task.Delay(seconds);

                        if (timesToWait != null && !string.IsNullOrEmpty(timesToWait.First()))
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested: " + timesToWait.First() + ". Milliseconds waitied: " + seconds.ToString(), "CallAmazonPostApiRequest", "Tracking only", null);
                        }
                        else
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested empty. Milliseconds waitied: " + seconds.ToString(), "CallAmazonPostApiRequest", "Tracking only", null);
                        }
                    }
                    catch (Exception ex)
                    {
                        LockAmazon = true;
                        await System.Threading.Tasks.Task.Delay(2000);

                        try
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested failed. Milliseconds waitied: 2000. See error:" + ex.ToString(), "CallAmazonPostApiRequest", "Tracking only", null);
                        }
                        catch (Exception e)
                        {
                            //nothing to do. best to keep going.
                        }

                    }
                    finally
                    {
                        LockAmazon = false;
                    }

                    //try call again
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                            var request = new HttpRequestMessage(HttpMethod.Post, EndPoint);
                            await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Post 285", serlializedJson);


                            request.Content = new StringContent(serlializedJson);
                            request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);

                            response = await client.SendAsync(request);

                            if (response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.TooManyRequests)
                            {
                                await System.Threading.Tasks.Task.Delay(4000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }
                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    //wait for 2 seconds and try again
                    await System.Threading.Tasks.Task.Delay(2000);

                    try
                    {
                        if (LockAmazon)
                        { await _semaphoreSlim.WaitAsync(); }

                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                            var request = new HttpRequestMessage(HttpMethod.Post, EndPoint);
                            await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Post 325", serlializedJson);

                            request.Content = new StringContent(serlializedJson);
                            request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);

                            response = await client.SendAsync(request);

                            if (response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.TooManyRequests)
                            {
                                await System.Threading.Tasks.Task.Delay(4000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }

                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //get refresh token
                    RetrieveData rd = new RetrieveData();
                    string refreshToken = await rd.GetRefreshToken(auth.ClientId);

                    //reauthorize and try again
                    APITokenCreation aPITokenCreation = new APITokenCreation();
                    TokenResponse token = await aPITokenCreation.GetTokenValue(refreshToken);
                    auth.AccessToken = token.access_token;

                    try
                    {
                        //save the new token back to the db
                        AllAccessTokens saveAccessToken = new AllAccessTokens();
                        saveAccessToken.AccessToken = token.access_token;
                        saveAccessToken.TokenExpirationTime = DateTime.Now.AddSeconds(token.expires_in);
                        saveAccessToken.ClientId = auth.ClientId;

                        SaveData sd = new SaveData();
                        await sd.UpdateAccessToken(saveAccessToken);
                    }
                    catch (Exception ex)
                    {
                        //nothing to do here. We want to process even if we don't save to the db.
                    }

                    try
                    {
                        if (LockAmazon)
                        { await _semaphoreSlim.WaitAsync(); }

                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                            var request = new HttpRequestMessage(HttpMethod.Post, EndPoint);
                            await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Post 389", serlializedJson);

                            request.Content = new StringContent(serlializedJson);
                            request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);

                            response = await client.SendAsync(request);
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }
                    return response;

                }
                else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500 && (int)response.StatusCode != 401 && (int)response.StatusCode != 429)
                {
                    try
                    {
                        Logging logging = new Logging();
                        LogError logError = new LogError();
                        logError.ErrorMessage = Convert.ToString((int)response.StatusCode);
                        logError.FailureMethod = "CallAmazonPostApi";
                        logError.ClientId = auth.ClientId;
                        logError.Parameters = JsonSerializer.Serialize(serlializedJson) + JsonSerializer.Serialize(auth) + EndPoint + JsonSerializer.Serialize(profileCode);
                        await logging.WriteToLog(logError);
                    }
                    catch(Exception ex)
                    {
                        //nothing to do. We want to keep going even if we don't save here.
                    }

                    //return failed response
                    return response;
                }
                else
                {
                    //return failed response
                    return response;
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CallAmazonPostApi";
                logError.ClientId = auth.ClientId;
                logError.Parameters = JsonSerializer.Serialize(serlializedJson) + JsonSerializer.Serialize(auth) + EndPoint + JsonSerializer.Serialize(profileCode);
                await logging.WriteToLog(logError);

                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        public async Task<HttpResponseMessage> CallAmazonGetApiRequest(string EndPoint, string MediaType, APIAuthorization auth, ClientProfileCodes profileCode, string serlializedJson = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                //removing this sleep for now since I'm making get requests run sequentially
                //sleep 1/100 of a second to delay speed of requests
                //await System.Threading.Tasks.Task.Delay(10);

                //get country api
                List<AzApiCountries> countriesList = CountriesSingleton.CountriesList();
                AzApiCountries thisCountry = countriesList.Where(x => x.Id == profileCode.CountryId).FirstOrDefault();

                int apiInt = thisCountry.AzApi;

                string url = "";

                switch (apiInt)
                {
                    case 1:
                        url = AzApiInfo.API1;
                        break;
                    case 2:
                        url = AzApiInfo.API2;
                        break;
                    case 3:
                        url = AzApiInfo.API3;
                        break;

                }

                try
                {
                    if (LockAmazon)
                    { await _semaphoreSlim.WaitAsync(); }

                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                        client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                        client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                        var request = new HttpRequestMessage(HttpMethod.Get, EndPoint);
                        //await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Get 489", serlializedJson);


                        request.Content = new StringContent(serlializedJson, Encoding.UTF8, MediaType);

                        response = await client.SendAsync(request);
                    }
                }
                catch (Exception ex)
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                }

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    try
                    {
                        LockAmazon = true;

                        //get value from header and try again
                        IEnumerable<string>? timesToWait = new List<string>();
                        var foundTimeToWait = response.Headers.TryGetValues("Retry-After", out timesToWait);

                        int seconds = 2000;

                        if (foundTimeToWait)
                        {
                            if (timesToWait != null && !string.IsNullOrEmpty(timesToWait.First()))
                            {
                                seconds = Convert.ToInt32(timesToWait.First()) * 1000;
                            }
                        }

                        await System.Threading.Tasks.Task.Delay(seconds);

                        if (timesToWait != null && !string.IsNullOrEmpty(timesToWait.First()))
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested: " + timesToWait.First() + ". Milliseconds waitied: " + seconds.ToString(), "CallAmazonGetApiRequest", "Tracking only", null);
                        }
                        else
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested empty. Milliseconds waitied: " + seconds.ToString(), "CallAmazonGetApiRequest", "Tracking only", null);
                        }
                    }
                    catch (Exception ex)
                    {
                        LockAmazon = true;
                        await System.Threading.Tasks.Task.Delay(2000);

                        try
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested failed. Milliseconds waitied: 2000. See error:" + ex.ToString(), "CallAmazonGetApiRequest", "Tracking only", null);
                        }
                        catch (Exception e)
                        {
                            //nothing to do. best to keep going.
                        }

                    }
                    finally
                    {
                        LockAmazon = false;
                    }

                    //try call again
                    try 
                    {
                        try
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(url);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                                client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                                client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                                var request = new HttpRequestMessage(HttpMethod.Get, EndPoint);
                                await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Get 597", serlializedJson);


                                request.Content = new StringContent(serlializedJson, Encoding.UTF8, MediaType);

                                response = await client.SendAsync(request);

                            }
                        }
                        catch(Exception ex)
                        {
                            await ErrorLogging.LogError("Failed on second get call. Exception: " + ex.ToString(), "Second Get Call", "");
                        }


                        if (response.IsSuccessStatusCode)
                        {
                            return response;
                        }
                        //try call last time
                        else
                        {
                            try
                            {
                                await System.Threading.Tasks.Task.Delay(4000);

                                using (HttpClient client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri(url);
                                    client.DefaultRequestHeaders.Accept.Clear();
                                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                                    client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                                    client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                                    var request = new HttpRequestMessage(HttpMethod.Get, EndPoint);
                                    await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Get failed on third get - 624", serlializedJson);


                                    request.Content = new StringContent(serlializedJson, Encoding.UTF8, MediaType);

                                    response = await client.SendAsync(request);
                                }

                            }
                            catch (Exception ex)
                            {
                                await ErrorLogging.LogError("Failed on third get call. Exception: " + ex.ToString(), "Third Get Call", "");
                            }

                            if (response.IsSuccessStatusCode)
                            {
                                return response;
                            }
                            else
                            {
                                await ErrorLogging.LogError("Failed on third Amazon get call. Seconds Requested empty. Milliseconds waitied: 4000. Error: " + response.StatusCode, "CallAmazonGetApiRequest", "Tracking only", null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }

                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    //wait for 2 seconds and try again
                    await System.Threading.Tasks.Task.Delay(2000);

                    try
                    {
                        if (LockAmazon)
                        { await _semaphoreSlim.WaitAsync(); }

                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                            var request = new HttpRequestMessage(HttpMethod.Get, EndPoint);
                            await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Get 612", serlializedJson);


                            request.Content = new StringContent(serlializedJson, Encoding.UTF8, MediaType);

                            response = await client.SendAsync(request);

                            if (response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.TooManyRequests)
                            {
                                await System.Threading.Tasks.Task.Delay(4000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }

                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //get refresh token
                    RetrieveData rd = new RetrieveData();
                    string refreshToken = await rd.GetRefreshToken(auth.ClientId);

                    //reauthorize and try again
                    APITokenCreation aPITokenCreation = new APITokenCreation();
                    TokenResponse token = await aPITokenCreation.GetTokenValue(refreshToken);
                    auth.AccessToken = token.access_token;

                    try
                    {
                        //save the new token back to the db
                        AllAccessTokens saveAccessToken = new AllAccessTokens();
                        saveAccessToken.AccessToken = token.access_token;
                        saveAccessToken.TokenExpirationTime = DateTime.Now.AddSeconds(token.expires_in);
                        saveAccessToken.ClientId = auth.ClientId;

                        SaveData sd = new SaveData();
                        await sd.UpdateAccessToken(saveAccessToken);
                    }
                    catch (Exception ex)
                    {
                        //nothing to do here. We want to process even if we don't save to the db.
                    }

                    try
                    {
                        if (LockAmazon)
                        { await _semaphoreSlim.WaitAsync(); }

                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                            var request = new HttpRequestMessage(HttpMethod.Get, EndPoint);
                            await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Get 674", serlializedJson);


                            request.Content = new StringContent(serlializedJson, Encoding.UTF8, MediaType);

                            response = await client.SendAsync(request);
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }

                    return response;

                }
                else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500 && (int)response.StatusCode != 401 && (int)response.StatusCode != 429)
                {
                    try
                    {
                        Logging logging = new Logging();
                        LogError logError = new LogError();
                        logError.ErrorMessage = Convert.ToString((int)response.StatusCode);
                        logError.FailureMethod = "CallAmazonGetApi";
                        logError.ClientId = auth.ClientId;
                        logError.Parameters = JsonSerializer.Serialize(serlializedJson) + JsonSerializer.Serialize(auth) + EndPoint + JsonSerializer.Serialize(profileCode);
                        await logging.WriteToLog(logError);
                    }
                    catch (Exception ex)
                    {
                        //nothing to do. We want to keep going even if we don't save here.
                    }

                    //return failed response
                    return response;
                }
                else
                {
                    //return failed response
                    return response;
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CallAmazonGetApi";
                logError.ClientId = auth.ClientId;
                logError.Parameters = JsonSerializer.Serialize(serlializedJson) + JsonSerializer.Serialize(auth) + EndPoint + JsonSerializer.Serialize(profileCode);
                await logging.WriteToLog(logError);

                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        public async Task<HttpResponseMessage> CallAmazonApiProfilesOnlyRequest(string EndPoint, string MediaType, APIAuthorization auth, int ApiToCall)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                string url = "";

                switch (ApiToCall)
                {
                    case 1:
                        url = AzApiInfo.API1;
                        break;
                    case 2:
                        url = AzApiInfo.API2;
                        break;
                    case 3:
                        url = AzApiInfo.API3;
                        break;

                }

                try
                {
                    if (LockAmazon)
                    { await _semaphoreSlim.WaitAsync(); }

                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                        client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);

                        var request = new HttpRequestMessage(HttpMethod.Get, EndPoint);

                        await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Profiles 769", "");

                        request.Content = new StringContent("", Encoding.UTF8, MediaType);

                        response = await client.SendAsync(request);
                    }
                }
                catch (Exception ex)
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                }

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    try
                    {
                        LockAmazon = true;

                        //get value from header and try again
                        IEnumerable<string>? timesToWait = new List<string>();
                        var foundTimeToWait = response.Headers.TryGetValues("Retry-After", out timesToWait);

                        int seconds = 2000;

                        if (foundTimeToWait)
                        {
                            if (timesToWait != null && !string.IsNullOrEmpty(timesToWait.First()))
                            {
                                seconds = Convert.ToInt32(timesToWait.First()) * 1000;
                            }
                        }

                        await System.Threading.Tasks.Task.Delay(seconds);

                        if (timesToWait != null && !string.IsNullOrEmpty(timesToWait.First()))
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested: " + timesToWait.First() + ". Milliseconds waitied: " + seconds.ToString(), "CallAmazonApiProfilesOnlyRequest", "Tracking only", null);
                        }
                        else
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested empty. Milliseconds waitied: " + seconds.ToString(), "CallAmazonApiProfilesOnlyRequest", "Tracking only", null);
                        }
                    }
                    catch (Exception ex)
                    {
                        LockAmazon = true;
                        await System.Threading.Tasks.Task.Delay(2000);

                        try
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested failed. Milliseconds waitied: 2000. See error:" + ex.ToString(), "CallAmazonApiProfilesOnlyRequest", "Tracking only", null);
                        }
                        catch (Exception e)
                        {
                           //nothing to do. best to keep going.
                        }

                    }
                    finally
                    {
                        LockAmazon = false;
                    }

                    //try call again
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);

                            var request = new HttpRequestMessage(HttpMethod.Get, EndPoint);
                            await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Profiles 849", "");

                            request.Content = new StringContent("", Encoding.UTF8, MediaType);

                            response = await client.SendAsync(request);

                            if (response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.TooManyRequests)
                            {
                                await System.Threading.Tasks.Task.Delay(4000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }
                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    //wait for 2 seconds and try again
                    await System.Threading.Tasks.Task.Delay(2000);

                    try
                    {
                        if (LockAmazon)
                        { await _semaphoreSlim.WaitAsync(); }

                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);

                            var request = new HttpRequestMessage(HttpMethod.Get, EndPoint);
                            await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Profiles 886", "");

                            request.Content = new StringContent("", Encoding.UTF8, MediaType);

                            response = await client.SendAsync(request);

                            if (response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.TooManyRequests)
                            {
                                await System.Threading.Tasks.Task.Delay(4000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }

                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //get refresh token
                    RetrieveData rd = new RetrieveData();
                    string refreshToken = await rd.GetRefreshToken(auth.ClientId);

                    //reauthorize and try again
                    APITokenCreation aPITokenCreation = new APITokenCreation();
                    TokenResponse token = await aPITokenCreation.GetTokenValue(refreshToken);
                    auth.AccessToken = token.access_token;

                    try
                    {
                        //save the new token back to the db
                        AllAccessTokens saveAccessToken = new AllAccessTokens();
                        saveAccessToken.AccessToken = token.access_token;
                        saveAccessToken.TokenExpirationTime = DateTime.Now.AddSeconds(token.expires_in);
                        saveAccessToken.ClientId = auth.ClientId;

                        SaveData sd = new SaveData();
                        await sd.UpdateAccessToken(saveAccessToken);
                    }
                    catch (Exception ex)
                    {
                        //nothing to do here. We want to process even if we don't save to the db.
                    }

                    try
                    {
                        if (LockAmazon)
                        { await _semaphoreSlim.WaitAsync(); }

                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);

                            var request = new HttpRequestMessage(HttpMethod.Get, EndPoint);
                            await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Profiles 948", "");

                            request.Content = new StringContent("", Encoding.UTF8, MediaType);

                            response = await client.SendAsync(request);
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }

                    return response;

                }
                else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500 && (int)response.StatusCode != 401 && (int)response.StatusCode != 429)
                {
                    try
                    {
                        Logging logging = new Logging();
                        LogError logError = new LogError();
                        logError.ErrorMessage = Convert.ToString((int)response.StatusCode);
                        logError.FailureMethod = "CallAmazonApiProfilesOnly";
                        logError.ClientId = auth.ClientId;
                        logError.Parameters = JsonSerializer.Serialize(auth) + EndPoint;
                        await logging.WriteToLog(logError);
                    }
                    catch (Exception ex)
                    {
                        //nothing to do. We want to keep going even if we don't save here.
                    }

                    //return failed response
                    return response;
                }
                else
                {
                    //return failed response
                    return response;
                }

            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CallAmazonApiProfilesOnly " + "Country: " + ApiToCall.ToString();
                logError.ClientId = auth.ClientId;
                logError.Parameters = JsonSerializer.Serialize(auth);
                await logging.WriteToLog(logError);

                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        public async Task<HttpResponseMessage> CallAmazonPutApiRequest(string EndPoint, string MediaType, APIAuthorization auth, ClientProfileCodes profileCode, string serlializedJson)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                //get country api
                List<AzApiCountries> countriesList = CountriesSingleton.CountriesList();
                AzApiCountries thisCountry = countriesList.Where(x => x.Id == profileCode.CountryId).FirstOrDefault();

                int apiInt = thisCountry.AzApi;

                string url = "";

                switch (apiInt)
                {
                    case 1:
                        url = AzApiInfo.API1;
                        break;
                    case 2:
                        url = AzApiInfo.API2;
                        break;
                    case 3:
                        url = AzApiInfo.API3;
                        break;

                }

                try
                {
                    if (LockAmazon)
                    { await _semaphoreSlim.WaitAsync(); }

                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                        client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                        client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                        var request = new HttpRequestMessage(HttpMethod.Put, EndPoint);
                        await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Put 1049", serlializedJson);


                        request.Content = new StringContent(serlializedJson);
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);

                        response = await client.SendAsync(request);
                    }
                }
                catch (Exception ex)
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                }

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    try
                    {
                        LockAmazon = true;

                        //get value from header and try again
                        IEnumerable<string>? timesToWait = new List<string>();
                        var foundTimeToWait = response.Headers.TryGetValues("Retry-After", out timesToWait);

                        int seconds = 2000;

                        if (foundTimeToWait)
                        {
                            if (timesToWait != null && !string.IsNullOrEmpty(timesToWait.First()))
                            {
                                seconds = Convert.ToInt32(timesToWait.First()) * 1000;
                            }
                        }

                        await System.Threading.Tasks.Task.Delay(seconds);

                        if (timesToWait != null && !string.IsNullOrEmpty(timesToWait.First()))
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested: " + timesToWait.First() + ". Milliseconds waitied: " + seconds.ToString(), "CallAmazonPostApiRequest", "Tracking only", null);
                        }
                        else
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested empty. Milliseconds waitied: " + seconds.ToString(), "CallAmazonPostApiRequest", "Tracking only", null);
                        }
                    }
                    catch (Exception ex)
                    {
                        LockAmazon = true;
                        await System.Threading.Tasks.Task.Delay(2000);

                        try
                        {
                            await ErrorLogging.LogError("Too Many Requests on Amazon call. Seconds Requested failed. Milliseconds waitied: 2000. See error:" + ex.ToString(), "CallAmazonPostApiRequest", "Tracking only", null);
                        }
                        catch (Exception e)
                        {
                            //nothing to do. best to keep going.
                        }

                    }
                    finally
                    {
                        LockAmazon = false;
                    }

                    //try call again
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                            var request = new HttpRequestMessage(HttpMethod.Put, EndPoint);
                            await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Put 1132", serlializedJson);

                            request.Content = new StringContent(serlializedJson);
                            request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);

                            response = await client.SendAsync(request);

                            if (response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.TooManyRequests)
                            {
                                await System.Threading.Tasks.Task.Delay(4000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }
                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    //wait for 2 seconds and try again
                    await System.Threading.Tasks.Task.Delay(2000);

                    try
                    {
                        if (LockAmazon)
                        { await _semaphoreSlim.WaitAsync(); }

                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                            var request = new HttpRequestMessage(HttpMethod.Put, EndPoint);
                            await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Put 1172", serlializedJson);

                            request.Content = new StringContent(serlializedJson);
                            request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);

                            response = await client.SendAsync(request);

                            if (response.StatusCode == HttpStatusCode.InternalServerError || response.StatusCode == HttpStatusCode.TooManyRequests)
                            {
                                await System.Threading.Tasks.Task.Delay(4000);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }

                    return response;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //get refresh token
                    RetrieveData rd = new RetrieveData();
                    string refreshToken = await rd.GetRefreshToken(auth.ClientId);

                    //reauthorize and try again
                    APITokenCreation aPITokenCreation = new APITokenCreation();
                    TokenResponse token = await aPITokenCreation.GetTokenValue(refreshToken);
                    auth.AccessToken = token.access_token;

                    try
                    {
                        //save the new token back to the db
                        AllAccessTokens saveAccessToken = new AllAccessTokens();
                        saveAccessToken.AccessToken = token.access_token;
                        saveAccessToken.TokenExpirationTime = DateTime.Now.AddSeconds(token.expires_in);
                        saveAccessToken.ClientId = auth.ClientId;

                        SaveData sd = new SaveData();
                        await sd.UpdateAccessToken(saveAccessToken);
                    }
                    catch (Exception ex)
                    {
                        //nothing to do here. We want to process even if we don't save to the db.
                    }

                    try
                    {
                        if (LockAmazon)
                        { await _semaphoreSlim.WaitAsync(); }

                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaType));

                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-ClientId", AzApiInfo.ClientId);
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.AccessToken);
                            client.DefaultRequestHeaders.Add("Amazon-Advertising-API-Scope", profileCode.ProfileCode);

                            var request = new HttpRequestMessage(HttpMethod.Put, EndPoint);
                            await ErrorLogging.AmazonApiLog("Log of API Call", "Amazon Put 1235", serlializedJson);

                            request.Content = new StringContent(serlializedJson);
                            request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);

                            response = await client.SendAsync(request);
                        }
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                    }
                    return response;

                }
                else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500 && (int)response.StatusCode != 401 && (int)response.StatusCode != 429)
                {
                    try
                    {
                        Logging logging = new Logging();
                        LogError logError = new LogError();
                        logError.ErrorMessage = Convert.ToString((int)response.StatusCode);
                        logError.FailureMethod = "CallAmazonPutApi";
                        logError.ClientId = auth.ClientId;
                        logError.Parameters = JsonSerializer.Serialize(serlializedJson) + JsonSerializer.Serialize(auth) + EndPoint + JsonSerializer.Serialize(profileCode);
                        await logging.WriteToLog(logError);
                    }
                    catch (Exception ex)
                    {
                        //nothing to do. We want to keep going even if we don't save here.
                    }

                    //return failed response
                    return response;
                }
                else
                {
                    //return failed response
                    return response;
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "CallAmazonPutApi";
                logError.ClientId = auth.ClientId;
                logError.Parameters = JsonSerializer.Serialize(serlializedJson) + JsonSerializer.Serialize(auth) + EndPoint + JsonSerializer.Serialize(profileCode);
                await logging.WriteToLog(logError);

                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }



    }
}
