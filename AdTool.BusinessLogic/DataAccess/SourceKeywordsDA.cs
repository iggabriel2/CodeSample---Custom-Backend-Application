using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class SourceKeywordsDA
    {
        public async Task<List<SourcedKeyword>> GetSourcedKeywordsList(string searchTerm)
        {
            List<SourcedKeyword> list = new List<SourcedKeyword>();
            try
            {

                var sql = "Select * from SourcedKeywords where Keyword LIKE @Keyword";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Keyword", searchTerm);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<SourcedKeyword>(sql, queryParameters)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetSourcedKeywordsList - SourceKeywordsDA.cs", string.Empty);
            }
            return list;
        }

        public async Task<SourcedKeyword> GetSourcedKeywordsById(int id)
        {
            SourcedKeyword result = new SourcedKeyword();
            try
            {

                var sql = "Select * from SourcedKeywords where KeywordId = @KeywordId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@KeywordId", id);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    result = (await connection.QueryAsync<SourcedKeyword>(sql, queryParameters)).FirstOrDefault() ?? new SourcedKeyword();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetSourcedKeywordsById - SourceKeywordsDA.cs", "id: " + id);
            }
            return result;
        }

        public async Task<int> InsertSourcedKeywords(SourcedKeyword keyword)
        {
            int sourcedKeywordId = 0;
            try
            {
                var sql = "INSERT INTO  dbo.SourcedKeywords (Keyword, AuthorId, IsExact, IsPhrase, IsBroad, IsNegativeExact, IsNegativePhrase, ExactBid, BroadBid, PhraseBid, IsExactActive, IsBroadActive, IsPhraseActive, Notes)" +
                    "VALUES (@Keyword, @AuthorId, @IsExact, @IsPhrase, @IsBroad, @IsNegativeExact, @IsNegativePhrase, @ExactBid, @BroadBid, @PhraseBid, @IsExactActive, @IsBroadActive, @IsPhraseActive, @Notes); SELECT CAST(SCOPE_IDENTITY() as int)";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    sourcedKeywordId = await connection.QuerySingleAsync<int>(sql, keyword);
                }
            }
            catch (Exception ex)
            {
                sourcedKeywordId = 0;
                await ErrorLogging.LogError(ex.ToString(), "InsertSourcedKeywords - SourceKeywordsDA.cs", JsonSerializer.Serialize(keyword));
            }
            return sourcedKeywordId;
        }

        public async Task<int> UpdateSourcedKeywords(SourcedKeyword keyword)
        {
            int result = 0;
            try
            {
                var sql = "UPDATE  dbo.SourcedKeywords SET Keyword = @Keyword, AuthorId = @AuthorId, IsExact = @IsExact, IsPhrase = @IsPhrase, IsBroad = @IsBroad, IsNegativeExact = @IsNegativeExact, IsNegativePhrase = @IsNegativePhrase, " +
                    "ExactBid = @ExactBid, BroadBid = @BroadBid, PhraseBid = @PhraseBid, IsExactActive = @IsExactActive, IsBroadActive = @IsBroadActive, IsPhraseActive = @IsPhraseActive, Notes =@Notes WHERE KeywordId = @KeywordId";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, keyword);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateSourcedKeywords - SourceKeywordsDA.cs", JsonSerializer.Serialize(keyword));
            }
            return result;
        }
    }
}
