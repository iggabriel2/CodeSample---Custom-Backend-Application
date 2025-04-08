using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AdTool.BusinessLogic.DataAccess
{
    public class QAPAlertsDA
    {
        public async Task<QAPAlerts> GetQAPAlerts(string alertType)
        {
            QAPAlerts record = new QAPAlerts();
            try
            {
                var sql = "SELECT * FROM QAPAlerts WHERE Type= @alertType AND IsActive = 1";
                var param = new { alertType = alertType };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    record = (await connection.QueryAsync<QAPAlerts>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetQAPAlerts - QAPAlertsDA.cs", "alert type: " + alertType);
            }
            return record;
        }
    }
}
