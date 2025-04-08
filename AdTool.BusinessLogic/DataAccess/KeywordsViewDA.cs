using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using AdTool.Entities.Logging;
using AdTool.Entities.View;
using Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class KeywordsViewDA
    {
        public async Task<int> GetKeywordSearchesHistoryCount(Guid clientId, bool currentBillingPeriod)
        {
            int count = 0;
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    count = (await connection.QueryAsync<int>("GetKeywordSearchHistoryCount", new { @ClientId = clientId, @CurrentBillingPeriod = currentBillingPeriod }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                count = -1;
                await ErrorLogging.LogError(ex.ToString(), "GetKeywordSearchHistoryCount - KeywordsViewDA.cs", "ClientId : " + clientId + "currentBillingPeriod : " + currentBillingPeriod);
            }
            return count;
        }
        public async Task<List<KeywordSearchHistoryView>> GetKeywordSearchesList(Guid clientId, bool? isSavedSearch)
        {
            List<KeywordSearchHistoryView> list = new List<KeywordSearchHistoryView>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<KeywordSearchHistoryView>("GetKeywordSearchHistoryList", new { @ClientId = clientId, @IsSavedSearch = isSavedSearch }, commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetSavedKeywordSearchesList - KeywordsViewDA.cs", "clientId : " + clientId + "isSavedSearch : " + isSavedSearch);
            }
            return list;
        }

        public async Task<List<KeywordHistoryView>> GetAzSpKeywordHistoryGridList(Guid clientId, int? countryId, int? productId, int? records)
        {
            List<KeywordHistoryView> list = new List<KeywordHistoryView>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<KeywordHistoryView>("GetAzSpKeywordHistoryGridList", new { @ClientId = clientId, @CountryId = countryId, @ProductId = productId, @Records = records }, commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetKeywordHistoryList - KeywordsViewDA.cs", "clientId : " + clientId + " productid : " + productId + "countryId: " + countryId + " records : " + records);
            }
            return list;
        }

        public async Task<string> GetSearchTermFromId(int id)
        {
            try
            {
                string searchTerm = "";

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    searchTerm = (await connection.QueryAsync<string>("select FriendlyName from KeywordsSearchTerms where id = @id", new { @id = id }, commandType: CommandType.Text)).FirstOrDefault();
                }

                return searchTerm;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
