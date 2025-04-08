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
    public class QAPQuestionsDA
    {
        public async Task<List<QAPQuestions>> GetListOfQAPQuestions()
        {
            List<QAPQuestions> list = new List<QAPQuestions>();
            try
            {
                var sql = "Select * from QAPQuestions WHERE IsActive = 1";
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    list = (await connection.QueryAsync<QAPQuestions>(sql)).ToList();
                }
            }
            catch (Exception ex)
            {
                await ErrorLogging.LogError(ex.ToString(), "GetListOfQAPQuestions - QAPQuestionsDA.cs", string.Empty);
            }

            return list;
        }
    }
}
