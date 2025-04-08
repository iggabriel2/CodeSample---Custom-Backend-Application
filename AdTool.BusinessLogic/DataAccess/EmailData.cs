using AdTool.Entities.Logging;
using Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using AdTool.BusinessLogic.Utilities;

namespace AdTool.BusinessLogic.DataAccess
{
    public class EmailData
    {
        public async Task<Entities.EmailSending.EmailTemplates> GetEmailTemplate(string TemplateName)
        {
            try
            {
                Entities.EmailSending.EmailTemplates temp = new Entities.EmailSending.EmailTemplates();
                using (var connection = new SqlConnection(DapperConnection.ConnectionString))
                {
                    temp = (await connection.QueryAsync<Entities.EmailSending.EmailTemplates>("GetEmailTemplates", new { @TemplateName = TemplateName }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                return temp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
