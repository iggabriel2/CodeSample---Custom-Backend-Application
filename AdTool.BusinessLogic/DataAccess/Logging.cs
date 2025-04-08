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

namespace AdTool.BusinessLogic.DataAccess
{
    public class Logging
    {
        public async Task<bool> WriteToLog(LogError logError)
        {
            //logging usage example:
            //Logging logging = new Logging();
            //LogError logError = new LogError();
            //logError.ErrorMessage = ex.ToString();
            //logError.FailureMethod = "UpdateAccessToken";
            //logError.ClientId = accessTokenFromDB.ClientId;
            //logError.Parameters = JsonSerializer.Serialize(accessTokenFromDB);
            //logging.WriteToLog(logError);

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("WriteToLog", logError, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                //nothing to do here
                return false;
            }
        }

        public async Task<bool> WriteToAmazonApiLog(LogError logError)
        {
            //logging usage example:
            //Logging logging = new Logging();
            //LogError logError = new LogError();
            //logError.ErrorMessage = ex.ToString();
            //logError.FailureMethod = "UpdateAccessToken";
            //logError.ClientId = accessTokenFromDB.ClientId;
            //logError.Parameters = JsonSerializer.Serialize(accessTokenFromDB);
            //logging.WriteToLog(logError);

            try
            {
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    var affectedRows = await connection.ExecuteAsync("WriteToAmazonApiLog", logError, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                //nothing to do here
                return false;
            }
        }


    }
}
