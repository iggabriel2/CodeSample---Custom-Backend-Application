using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Create.Keywords
{
    //request
    public class APIKeyword
    {
        public string campaignId { get; set; }
        public string matchType { get; set; }
        public string state { get; set; }
        public decimal bid { get; set; }
        public string adGroupId { get; set; }
        public string keywordText { get; set; }
    }

    public class KeywordRequestRoot
    {
        public List<APIKeyword> keywords { get; set; }
        public KeywordRequestRoot()
        {
            keywords = new List<APIKeyword>();
        }
    }



    //response

    public class BiddingError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public Cause cause { get; set; }
        public string upperLimit { get; set; }
        public string lowerLimit { get; set; }
        public string message { get; set; }
    }

    public class BillingError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }

    public class Cause
    {
        public string location { get; set; }
        public string trigger { get; set; }
    }

    public class DuplicateValueError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }

    public class EntityNotFoundError
    {
        public string reason { get; set; }
        public string entityType { get; set; }
        public Cause cause { get; set; }
        public string entityId { get; set; }
        public string message { get; set; }
    }

    public class EntityQuotaError
    {
        public string reason { get; set; }
        public string quotaScope { get; set; }
        public string entityType { get; set; }
        public string quota { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }

    public class EntityStateError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public string entityType { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }

    public class Error
    {
        public int index { get; set; }
        public List<Error2> errors { get; set; }
    }

    public class Error2
    {
        public string errorType { get; set; }
        public ErrorValue errorValue { get; set; }
    }

    public class ErrorValue
    {
        public EntityStateError entityStateError { get; set; }
        public MissingValueError missingValueError { get; set; }
        public BiddingError biddingError { get; set; }
        public DuplicateValueError duplicateValueError { get; set; }
        public RangeError rangeError { get; set; }
        public ParentEntityError parentEntityError { get; set; }
        public OtherError otherError { get; set; }
        public ThrottledErrorKey throttledError { get; set; }
        public EntityNotFoundError entityNotFoundError { get; set; }
        public TargetingClauseSetupError targetingClauseSetupError { get; set; }
        public LocaleError localeError { get; set; }
        public MalformedValueError malformedValueError { get; set; }
        public BillingError billingError { get; set; }
        public EntityQuotaError entityQuotaError { get; set; }
        public InternalServerError internalServerError { get; set; }
    }

    public class ExtendedData
    {
        public DateTime lastUpdateDateTime { get; set; }
        public string servingStatus { get; set; }
        public List<ServingStatusDetailKey> servingStatusDetails { get; set; }
        public DateTime creationDateTime { get; set; }
        public ExtendedData()
        {
            servingStatusDetails = new List<ServingStatusDetailKey>();
        }
    }

    public class InternalServerError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }

    public class Keyword
    {
        public string keywordId { get; set; }
        public string nativeLanguageKeyword { get; set; }
        public string nativeLanguageLocale { get; set; }
        public string campaignId { get; set; }
        public string matchType { get; set; }
        public string state { get; set; }
        public decimal bid { get; set; }
        public string adGroupId { get; set; }
        public string keywordText { get; set; }
        public ExtendedData extendedData { get; set; }
    }

    public class Keywords
    {
        public List<SuccessKey> success { get; set; }
        public List<Error> error { get; set; }
    }

    public class LocaleError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }

    public class MalformedValueError
    {
        public string reason { get; set; }
        public string fragment { get; set; }
        public string marketplace { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }

    public class MissingValueError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }

    public class OtherError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }

    public class ParentEntityError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }

    public class RangeError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public List<string> allowed { get; set; }
        public Cause cause { get; set; }
        public string upperLimit { get; set; }
        public string lowerLimit { get; set; }
        public string message { get; set; }
    }

    public class KeywordResponseRoot
    {
        public Keywords keywords { get; set; }
        public KeywordResponseRoot()
        {
            keywords = new Keywords();
        }
    }

    public class ServingStatusDetailKey
    {
        public string name { get; set; }
        public string helpUrl { get; set; }
        public string message { get; set; }
    }

    public class SuccessKey
    {
        public string keywordId { get; set; }
        public int index { get; set; }
        public Keyword keyword { get; set; }
        public SuccessKey()
        {
            keyword = new Keyword();
        }
    }

    public class TargetingClauseSetupError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }

    public class ThrottledErrorKey
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
    }
}
