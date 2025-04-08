using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class KeywordsSearchTermsDA
    {
        public async Task<List<KeywordsSearchTerms>> GetKeywordsSearchTermsList(string authorName)
        {
            List<KeywordsSearchTerms> list = new List<KeywordsSearchTerms>();
            try
            {

                var sql = "Select * from KeywordsSearchTerms where FriendlyName LIKE @AuthorName";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@AuthorName", authorName);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<KeywordsSearchTerms>(sql, queryParameters)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetKeywordsSearchTermsList - KeywordsSearchTermsDA.cs", string.Empty);
            }
            return list;
        }

        public async Task<KeywordsSearchTerms> GetKeywordsSearchTermsById(int id)
        {
            KeywordsSearchTerms list = new KeywordsSearchTerms();
            try
            {

                var sql = "Select * from KeywordsSearchTerms where Id = @id";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@id", id);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<KeywordsSearchTerms>(sql, queryParameters)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetKeywordsSearchTermsList - KeywordsSearchTermsDA.cs", string.Empty);
            }
            return list;
        }
    }
}
