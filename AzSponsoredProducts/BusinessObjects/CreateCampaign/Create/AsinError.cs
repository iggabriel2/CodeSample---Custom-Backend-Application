using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.AsinError
{
    public class Cause
    {
        public string location { get; set; }
        public string trigger { get; set; }
    }

    public class TriggerItem
    {
            public string type { get; set; }
            public string value { get; set; }
    }

    public class Error
    {
        public string errorType { get; set; }
        public ErrorValue errorValue { get; set; }
        public Error() { 
            errorValue = new ErrorValue();
        }
    }

    public class ErrorValue
    {
        public TargetingClauseSetupError targetingClauseSetupError { get; set; }
        public ErrorValue() { 
            targetingClauseSetupError = new TargetingClauseSetupError();
        }
    }

    public class AsinErrorRoot
    {
        public List<Error> errors { get; set; }
        public int index { get; set; }
        public AsinErrorRoot() {
            errors = new List<Error>();
        }
    }

    public class TargetingClauseSetupError
    {
        public Cause cause { get; set; }
        public string message { get; set; }
        public string reason { get; set; }
        public TargetingClauseSetupError()
        {
            cause = new Cause();
        }
    }


}
