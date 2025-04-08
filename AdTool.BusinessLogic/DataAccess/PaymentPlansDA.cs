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
    public class PaymentPlansDA
    {
        public async Task<List<PaymentPlans>> GetPaymentPlansList(UIMessage message)
        {
            List<PaymentPlans> list = new List<PaymentPlans>();
            try
            {
                var sql = "SELECT * FROM PaymentPlans";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<PaymentPlans>(sql)).ToList();
                }
            }
            catch (Exception ex)
            {
                message.ErrorMessages.Add("An error occured while processing this request.");
                await ErrorLogging.LogError(ex.ToString(), "GetPaymentPlansList - PaymentPlansDA.cs", "");
            }
            return list;
        }
    }
}
