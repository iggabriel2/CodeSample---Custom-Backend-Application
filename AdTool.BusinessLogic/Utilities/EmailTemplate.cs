using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.EmailSending;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.Utilities
{
    public class EmailTemplate
    {
        public string Subject { get; set; }
        public string Body { get; set; }

        public async static Task<EmailTemplate> GetTemplate(string emailType, List<EmailToken>? Tokens = null)
        {
            EmailTemplate template = new EmailTemplate();

            EmailData data = new EmailData();
            EmailTemplates dbTemplateValue = await data.GetEmailTemplate(emailType);
            template.Subject = dbTemplateValue.TemplateSubject;
            template.Body = await GetBodyValue(dbTemplateValue.TemplateBodyFileName, Tokens);

            return template;
        }

        public async static Task<String> GetBodyValue(string FileName, List<EmailToken>? Tokens = null) 
        {
            var path = "EmailTemplates\\" + FileName + ".txt";
            string readText = await File.ReadAllTextAsync(path);

            foreach (var token in Tokens)
            {
                readText = readText.Replace("||" + token.TokenName + "||", token.TokenValue);
            }

            return readText;
        }
    }
}
