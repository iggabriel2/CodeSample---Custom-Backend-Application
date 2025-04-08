using AdTool.Entities.AzSp.ClientAuthorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Utils
{
    public class CosmosUtils
    {
        private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(0);

        public async Task<string> CosmosQuery(string EndPoint, string MediaType, APIAuthorization auth, ClientProfileCodes profileCode, string serlializedJson = "")
        {
            string response = "";
            try
            {
                await _semaphoreSlim.WaitAsync();
                //await System.Threading.Tasks.Task.Delay(1000);
                response = await ProcessCosmos();
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

        public async Task<string> ProcessCosmos()
        {
            return "A";
        }
    }
}
