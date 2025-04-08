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
    public class QAPVideosDA
    {
        public async Task<List<QAPVideos>> GetListOfQAPVideos()
        {
            List<QAPVideos> list = new List<QAPVideos>();
            try
            {
                var sql = "Select * from QAPVideos WHERE IsActive = 1";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<QAPVideos>(sql)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetListOfQAPVideos - QAPVideosDA.cs", string.Empty);
            }

            return list;
        }
    }
}
