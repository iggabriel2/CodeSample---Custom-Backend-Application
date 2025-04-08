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
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace AdTool.BusinessLogic.DataAccess
{
    public class ReportRunByClientLoggingDA
    {
        public async Task<List<ReportRunByClientLogging>> GetReportRunByClientLoggingByClientId(Guid clientId)
        {
            List<ReportRunByClientLogging> list = new List<ReportRunByClientLogging>();
            try
            {
                var sql = "SELECT * FROM ReportRunByClientLogging WHERE AzClientId = @ClientId";
                var param = new { ClientId = clientId };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<ReportRunByClientLogging>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetReportRunByClientLoggingByClientId - ReportRunByClientLoggingDA.cs", "clientId : " + clientId);
            }
            return list;
        }

    }
}
