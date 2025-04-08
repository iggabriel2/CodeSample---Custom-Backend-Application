using AdTool.BusinessLogic.Utilities;
using AdTool.Entities.Edit;
using AdTool.Entities.Logging;
using Configuration;
using Dapper;
using System.Data.SqlClient;


namespace AdTool.BusinessLogic.DataAccess
{
    public class PromoCodeDA
    {
        public async Task<List<PromoCode>> GetPromoCodeList(UIMessage message)
        {
            List<PromoCode> list = new List<PromoCode>();
            try
            {
                var sql = "SELECT * FROM PromoCode";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<PromoCode>(sql)).ToList();
                }
            }
            catch (Exception ex)
            {
                message.ErrorMessages.Add("An error occured while processing this request.");
                await ErrorLogging.LogError(ex.ToString(), "GetPromoCodeList - PromoCodeDA.cs", "");
            }
            return list;
        }

        public async Task<PromoCode> GetPromoCode(int id)
        {
            PromoCode code = new PromoCode();
            try
            {
                var sql = "SELECT * FROM PromoCode WHERE Id = @Id";
                var param = new { Id = id };
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    code = (await connection.QueryAsync<PromoCode>(sql, param)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetPromoCode - PromoCodeDA.cs", "id : " + id);
            }
            return code;
        }
    }
}
