using AdTool.AzSponsoredProducts.BusinessObjects.General;
using AdTool.AzSponsoredProducts.BusinessObjects.Keyword;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.Logging;
using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Data.KeywordManagement
{
    public class KeywordFunctions
    {
        public async Task<bool> ApplyNegativeOneOff(SaveSummaryReportAction negative, Guid clientId)
        {
            try
            {
                negative.ClientId = clientId;

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("BlockNegativeOneOff", negative, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "ApplyNegativeOneOff";
                logError.ClientId = clientId;
                logError.Parameters = JsonSerializer.Serialize(negative);
                await logging.WriteToLog(logError);
            }
            return true;
        }

        public async Task<bool> MarkSearchTermReviewed(SaveSummaryReportAction reviewedStatus, Guid clientId)
        {
            try
            {
                reviewedStatus.ClientId = clientId;

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("MarkSearchTermReviewed", reviewedStatus, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "MarkSearchTermReviewed";
                logError.ClientId = clientId;
                logError.Parameters = JsonSerializer.Serialize(reviewedStatus);
                await logging.WriteToLog(logError);
            }
            return true;
        }

        public async Task<string> GetKeywordText(string KeywordId, int CountryId, Guid ClientId)
        {
            try
            {
                string keywordText = "";

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    keywordText = (await connection.QueryAsync<string>("GetKeywordText", new { @KeywordId = KeywordId, @CountryId = CountryId, @ClientId = ClientId }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }

                return keywordText;
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "GetKeywordText";
                logError.ClientId = ClientId;
                logError.Parameters = KeywordId + " " + CountryId + " " + ClientId.ToString();
                await logging.WriteToLog(logError);

                return "";
            }
        }
    }
}
