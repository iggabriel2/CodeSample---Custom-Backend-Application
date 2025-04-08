using AdTool.Entities.Edit;
using AdTool.Entities.Edit.Auth;
using AdTool.Entities.Logging;
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
    public class QAPSupportTicketsDA
    {
        public async Task<int> InsertQAPSupportTickets(QAPSupportTickets ticket)
        {
            int ticketId = 0;
            try
            {
                var sql = "INSERT INTO QAPSupportTickets (SubjectLine ,RequestDescription , AdditionalEmailAddresses, UserId) " +
                    "VALUES (@SubjectLine ,@RequestDescription , @AdditionalEmailAddresses, @UserId); SELECT CAST(SCOPE_IDENTITY() as int)";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    ticketId = await connection.QuerySingleAsync<int>(sql, ticket);
                }
            }
            catch (Exception ex)
            {
                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ex.ToString();
                logError.FailureMethod = "QAPSupportTickets - QAPSupportTicketsDA.cs";
                logError.Parameters = JsonSerializer.Serialize(ticket);
                await logging.WriteToLog(logError);
            }
            return ticketId;
        }
    }
}
