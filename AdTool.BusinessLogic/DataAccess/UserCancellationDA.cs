using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using AdTool.Entities.Edit.Auth;
using Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.DataAccess
{
    public class UserCancellationDA
    {
        public async Task<int> InsertUserCancellation(UserCancellation cancellation)
        {
            int cancellationid = 0;
            try
            {
                var sql = "INSERT INTO UserCancellation (UserId, CancellationReason) " +
                    "VALUES (@UserId, @CancellationReason); SELECT CAST(SCOPE_IDENTITY() as int)";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    cancellationid = await connection.QuerySingleAsync<int>(sql, cancellation);
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "InsertUserCancellation - UserCancellationDA.cs", JsonSerializer.Serialize(cancellation));
            }
            return cancellationid;
        }
    }
}
