using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    public class BiddingError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public Cause cause { get; set; }
        public string upperLimit { get; set; }
        public string lowerLimit { get; set; }
        public string message { get; set; }
        public BiddingError()
        {
            cause = new Cause();
        }
    }

    public class BillingError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public BillingError()
        {
            cause = new Cause();
        }
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
        public DuplicateValueError()
        {
            cause = new Cause();
        }
    }

    public class EntityNotFoundError
    {
        public string reason { get; set; }
        public string entityType { get; set; }
        public Cause cause { get; set; }
        public string entityId { get; set; }
        public string message { get; set; }
        public EntityNotFoundError()
        {
            cause = new Cause();
        }
    }

    public class EntityQuotaError
    {
        public string reason { get; set; }
        public string quotaScope { get; set; }
        public string entityType { get; set; }
        public string quota { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public EntityQuotaError()
        {
            cause = new Cause();
        }
    }

    public class EntityStateError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public string entityType { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public EntityStateError()
        {
            cause = new Cause();
        }
    }

    public class Error
    {
        public int index { get; set; }
        public List<Error> errors { get; set; }
        public Error()
        {
            errors = new List<Error>();
        }
    }

    public class Error2
    {
        public string errorType { get; set; }
        public ErrorValue errorValue { get; set; }
        public Error2()
        {
            errorValue = new ErrorValue();
        }
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
        public ThrottledError throttledError { get; set; }
        public EntityNotFoundError entityNotFoundError { get; set; }
        public TargetingClauseSetupError targetingClauseSetupError { get; set; }
        public LocaleError localeError { get; set; }
        public MalformedValueError malformedValueError { get; set; }
        public BillingError billingError { get; set; }
        public EntityQuotaError entityQuotaError { get; set; }
        public InternalServerError internalServerError { get; set; }
        public ErrorValue()
        {
            entityStateError = new EntityStateError();
            missingValueError = new MissingValueError();
            biddingError = new BiddingError();
            duplicateValueError = new DuplicateValueError();
            rangeError = new RangeError();
            parentEntityError = new ParentEntityError();
            otherError = new OtherError();
            throttledError = new ThrottledError();
            entityNotFoundError = new EntityNotFoundError();
            targetingClauseSetupError = new TargetingClauseSetupError();
            localeError = new LocaleError();
            malformedValueError = new MalformedValueError();
            billingError = new BillingError();
            entityQuotaError = new EntityQuotaError();
            internalServerError = new InternalServerError();
        }
    }

    public class ExtendedData
    {
        public DateTime lastUpdateDateTime { get; set; }
        public string servingStatus { get; set; }
        public List<ServingStatusDetail> servingStatusDetails { get; set; }
        public DateTime creationDateTime { get; set; }
        public ExtendedData()
        {
            servingStatusDetails = new List<ServingStatusDetail>();
        }
    }

    public class InternalServerError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public InternalServerError()
        {
            cause = new Cause();
        }
    }

    public class KeywordResponseValues
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
        public KeywordResponseValues()
        {
            extendedData = new ExtendedData();
        }
    }

    public class Keywords
    {
        public List<Success> success { get; set; }
        public List<Error> error { get; set; }
        public Keywords()
        {
            success = new List<Success>();
            error = new List<Error>();
        }
    }

    public class LocaleError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public LocaleError()
        {
            cause = new Cause();
        }
    }

    public class MalformedValueError
    {
        public string reason { get; set; }
        public string fragment { get; set; }
        public string marketplace { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public MalformedValueError()
        {
            cause = new Cause();
        }
    }

    public class MissingValueError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public MissingValueError()
        {
            cause = new Cause();
        }
    }

    public class OtherError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public OtherError()
        {
            cause = new Cause();
        }
    }

    public class ParentEntityError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public ParentEntityError()
        {
            cause = new Cause();
        }
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
        public RangeError()
        {
            cause = new Cause();
            allowed = new List<string>();
        }
    }

    public class KeywordUpdateResponse
    {
        public Keywords keywords { get; set; }
        public KeywordUpdateResponse()
        {
            keywords = new Keywords();
        }
    }

    public class ServingStatusDetail
    {
        public string name { get; set; }
        public string helpUrl { get; set; }
        public string message { get; set; }
    }

    public class Success
    {
        public string keywordId { get; set; }
        public int index { get; set; }
        public KeywordResponseValues keyword { get; set; }
        public Success()
        {
            keyword = new KeywordResponseValues();
        }
    }

    public class TargetingClauseSetupError
    {
        public string reason { get; set; }
        public string marketplace { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public TargetingClauseSetupError()
        {
            cause = new Cause();
        }
    }

    public class ThrottledError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public ThrottledError()
        {
            cause = new Cause();
        }
    }
}
