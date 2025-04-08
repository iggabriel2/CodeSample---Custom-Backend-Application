using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using AdTool.Entities.Logging;
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
    public class PaymentSchedulesDA
    {
        public async Task<List<PaymentSchedules>> GetPaymentSchedulesList(UIMessage message)
        {
            List<PaymentSchedules> list = new List<PaymentSchedules>();
            try
            {
                var sql = "SELECT * FROM PaymentSchedules";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<PaymentSchedules>(sql)).ToList();
                }
            }
            catch (Exception ex)
            {
                message.ErrorMessages.Add("An error occured while processing this request.");
                await ErrorLogging.LogError(ex.ToString(), "GetPaymentSchedulesList - PaymentSchedulesDA", "");
            }
            return list;
        }
    }
}
