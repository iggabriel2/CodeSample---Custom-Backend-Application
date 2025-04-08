using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using AdTool.Entities.Keywords;
using Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class KeywordSearchHistoryDA
    {
        public async Task<int> InsertKeywordSearchHistory(KeywordSearchHistory searchRecord)
        {

            int id = 0;
            try
            {
                var sql = "INSERT INTO KeywordSearchHistory (ClientId, SearchTermId, Date, IsSavedSearch) " +
                    "VALUES (@ClientId, @SearchTermId, @Date, @IsSavedSearch); SELECT CAST(SCOPE_IDENTITY() as int)";

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    id = await connection.QuerySingleAsync<int>(sql, searchRecord);
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "InsertKeywordSearchHistory - KeywordSearchHistoryDA.cs", JsonSerializer.Serialize(searchRecord));
            }
            return id;
        }

        public async Task<bool> UpdateKeywordSearchHistory(int historyId, Guid clientId, string savedSearchName)
        {

            bool isUpdated = true;
            try
            {
                var sql = "UPDATE KeywordSearchHistory SET IsSavedSearch = 1, SavedSearchName =  @SavedSearchName where ClientId = @ClientId AND Id = @HistoryId";
                var param = new DynamicParameters();
                param.Add("@HistoryId", historyId);
                param.Add("@ClientId", clientId);
                param.Add("@SavedSearchName", savedSearchName);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, param);
                }
            }
            catch (Exception ex)
            {
                isUpdated = false;
                await ErrorLogging.LogError(ex.ToString(), "UpdateKeywordSearchHistory - KeywordSearchHistoryDA.cs", " historyId : " + historyId + " clientId: " + clientId + " savedSearchName : " + savedSearchName);
            }
            return isUpdated;
        }

    }
}
