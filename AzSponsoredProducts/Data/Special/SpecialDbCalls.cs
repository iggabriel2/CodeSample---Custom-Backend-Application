using AdTool.AzSponsoredProducts.BusinessObjects.Reports;
using AdTool.AzSponsoredProducts.BusinessObjects.SearchTermManagement;
using AdTool.AzSponsoredProducts.BusinessObjects.Special;
using AdTool.AzSponsoredProducts.Utils;
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
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Data.Special
{
    public class SpecialDbCalls
    {
        public async Task<List<string>> GetAllAsins(string compressedSearchTerm)
        {
            List<string> asins = new List<string>();
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    asins = (await connection.QueryAsync<string>("GetAllAsins", new { @compressedSearchTerm = compressedSearchTerm }, commandType: CommandType.StoredProcedure)).ToList();
                }
                return asins;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> GetSearchTermId(string compressedSearchTerm)
        {
            int searchTermId = 0;
            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    searchTermId = (await connection.QueryAsync<int>("GetSearchTermId", new { @compressedSearchTerm = compressedSearchTerm }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return searchTermId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<bool> SaveNewKeywords(List<SpecialKeywords> SpecialKeywords)
        {
            try
            {

                LinqToDataTableUtil linqToDataTableUtil = new LinqToDataTableUtil();
                DataTable dt = linqToDataTableUtil.LinqToDataTable<SpecialKeywords>(SpecialKeywords);

                using (SqlBulkCopy bcopy = new SqlBulkCopy(DapperConnection.ConnectionString))
                {
                    bcopy.BulkCopyTimeout = 120;
                    bcopy.DestinationTableName = "dbo.zSpecialKeywordsTemp";
                    SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("KeywordSearchTermId", "KeywordSearchTermId");
                    SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("Keyword", "Keyword");
                    SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("TypeId", "TypeId");
                    SqlBulkCopyColumnMapping mapping6 = new SqlBulkCopyColumnMapping("SourceId", "SourceId");

                    bcopy.ColumnMappings.Add(mapping2);
                    bcopy.ColumnMappings.Add(mapping3);
                    bcopy.ColumnMappings.Add(mapping4);
                    bcopy.ColumnMappings.Add(mapping6);
                    bcopy.WriteToServer(dt);
                }

                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("ReconcileSpecialKeywordsTemp", commandType: CommandType.StoredProcedure);
                }

                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }

        }
    }

}
