using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
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
    public class ActionRequiredDA
    {
        public async Task<ActionRequired> GetActionRequiredById(Guid clientId, int id)
        {
            ActionRequired actionResult = new ActionRequired();
            try
            {
                var sqlStatement = "Select * from ActionRequired where ClientId = @ClientId and Id = @Id";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@ClientId", clientId);
                queryParameters.Add("@Id", id);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    actionResult = (await connection.QueryAsync<ActionRequired>(sqlStatement, queryParameters)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetActionRequiredById - ActionRequiredDA.cs", " clientId : " + clientId + " id: " + id);
            }
            return actionResult;
        }

        public async Task<int> UpdateActionRequiredResolved(Guid clientId, int id, bool resolved)
        {

            int result = 0;
            try
            {
                var sql = "UPDATE ActionRequired " +
                    "SET Resolved = @Resolved " +
                    "WHERE Id = @Id AND ClientId = @ClientId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Resolved", resolved);
                queryParameters.Add("@Id", id);
                queryParameters.Add("@ClientId", clientId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateActionRequiredResolved - ActionRequiredDA.cs", "clientId : " + clientId + " , id: " + id + " resolved: " + resolved);
            }
            return result;
        }

        public async Task<int> UpdateActionRequiredReconciledStatus(Guid clientId, int id, bool reconciled)
        {

            int result = 0;
            try
            {
                var sql = "UPDATE ActionRequired " +
                    "SET Reconcile = @Reconcile " +
                    "WHERE Id = @Id AND ClientId = @ClientId";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Reconcile", reconciled);
                queryParameters.Add("@Id", id);
                queryParameters.Add("@ClientId", clientId);
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, queryParameters);
                }
            }
            catch (Exception ex)
            {
                result = -1;
                await ErrorLogging.LogError(ex.ToString(), "UpdateActionRequiredReconciledStatus - ActionRequiredDA.cs", "clientId : " + clientId + " , id: " + id + " reconcile: " + reconciled);
            }
            return result;
        }


    }
}
