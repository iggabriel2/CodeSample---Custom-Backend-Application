using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.AmazonAPI.General
{
    public class LocationFetcher
    {
        public async Task<CountryResponse> GetCountry(string ipAddress)
        {
            string country = "";
            string URL = "http://api.ipstack.com/" + ipAddress;
            string urlParameters = "?access_key=88f0caee92a0fc4efe4ea6bae533778c";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(urlParameters).Result;

            CountryResponse? countryResponse = new CountryResponse();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    string res = "";
                    using (HttpContent content = response.Content)
                    {
                        Task<string> result = content.ReadAsStringAsync();
                        res = result.Result;
                    }
                    countryResponse = JsonConvert.DeserializeObject<CountryResponse>(res);

                    if (countryResponse == null)
                    {
                        countryResponse.country_name = "United States";
                    }
                }
                catch (Exception ex)
                {
                    countryResponse.country_name = "United States";
                }
            }
            client.Dispose();

            return countryResponse;
        }

        public class CountryResponse
        {
            public string country_name { get; set; }
        }
    }
}
