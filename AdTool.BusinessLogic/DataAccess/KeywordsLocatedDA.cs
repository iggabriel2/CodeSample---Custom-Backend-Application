using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using AdTool.Entities.View;
using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class KeywordsLocatedDA
    {

        public async Task<List<string>> GetKeywordsLocatedBySearchTermIdAsString(int searchTermId)
        {
            List<string> list = new List<string>();
            try
            {
                var sql = "SELECT Keyword FROM KeywordsLocated WHERE KeywordSearchTermId = @SearchTermId AND (TypeId = 1 OR TypeId = 3)";
                var param = new DynamicParameters();
                param.Add("@SearchTermId", searchTermId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<string>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetKeywordsLocatedBySearchTermIdTypeId - KeywordsLocatedDA.cs", "searchTermId : " + searchTermId);
            }
            return list;
        }

        public async Task<List<string>> GetASINsLocatedBySearchTermIdAsString(int searchTermId, int keywordType)
        {
            List<string> list = new List<string>();
            try
            {
                var sql = "SELECT Keyword FROM KeywordsLocated WHERE KeywordSearchTermId = @SearchTermId AND TypeId = @TypeId";
                var param = new DynamicParameters();
                param.Add("@SearchTermId", searchTermId);
                param.Add("@TypeId", keywordType);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<string>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetASINsLocatedBySearchTermIdAsString - KeywordsLocatedDA.cs", "searchTermId : " + searchTermId);
            }
            return list;
        }

        public async Task<List<KeywordsLocated>> GetKeywordsLocatedBySearchTermId(int searchTermId)
        {
            List<KeywordsLocated> list = new List<KeywordsLocated>();
            try
            {
                var sql = "SELECT * FROM KeywordsLocated WHERE KeywordSearchTermId = @SearchTermId";

                var param = new DynamicParameters();
                param.Add("@SearchTermId", searchTermId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<KeywordsLocated>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetKeywordsLocatedBySearchTermIdTypeId - KeywordsLocatedDA.cs", "searchTermId : " + searchTermId);
            }
            return list;
        }
    }
}
