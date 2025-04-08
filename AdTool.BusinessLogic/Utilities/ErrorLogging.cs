using AdTool.BusinessLogic.DataAccess;
using AdTool.Entities.Logging;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.Utilities
{
    public class ErrorLogging
    {
        public async static Task LogError(string ErrorMessage, string FailureMethod, string Parameters, Guid? ClientId = null)
        {
            try
            {
                Guid ClientIdToLog = Guid.Empty;
                if (ClientId != null)
                {
                    ClientIdToLog = ClientId.Value;
                }

                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ErrorMessage;
                logError.FailureMethod = FailureMethod;
                logError.ClientId = ClientIdToLog;
                logError.Parameters = Parameters;
                await logging.WriteToLog(logError);
            }
            catch (Exception ex)
            {
                //nothing to do. Better to keep going than fail on logging.
            }
           
        }

        public async static Task AmazonApiLog(string ErrorMessage, string FailureMethod, string Parameters, Guid? ClientId = null)
        {
            try
            {
                Guid ClientIdToLog = Guid.Empty;
                if (ClientId != null)
                {
                    ClientIdToLog = ClientId.Value;
                }

                Logging logging = new Logging();
                LogError logError = new LogError();
                logError.ErrorMessage = ErrorMessage;
                logError.FailureMethod = FailureMethod;
                logError.ClientId = ClientIdToLog;
                logError.Parameters = Parameters;
                await logging.WriteToAmazonApiLog(logError);
            }
            catch (Exception ex)
            {
                //nothing to do. Better to keep going than fail on logging.
            }

        }
    }
}
