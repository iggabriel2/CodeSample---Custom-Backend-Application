using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.CreateCampaign.Update
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

    public class BudgetCampaign
    {
        public string budgetType { get; set; }
        public int budget { get; set; }
        public int effectiveBudget { get; set; }
    }

    public class BudgetError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string upperLimit { get; set; }
        public string lowerLimit { get; set; }
        public string message { get; set; }
        public BudgetError()
        {
            cause = new Cause();
        }
    }

    public class CampaignUpdateValues
    {
        public string portfolioId { get; set; }
        public string endDate { get; set; }
        public string campaignId { get; set; }
        public string name { get; set; }
        public string targetingType { get; set; }
        public string state { get; set; }
        public DynamicBiddingCampaign dynamicBidding { get; set; }
        public string startDate { get; set; }
        public BudgetCampaign budget { get; set; }
        public TagsCampaign tags { get; set; }
        public ExtendedData extendedData { get; set; }
        public CampaignUpdateValues()
        {
            dynamicBidding = new DynamicBiddingCampaign();
            budget = new BudgetCampaign();
            tags = new TagsCampaign();
            extendedData = new ExtendedData();
        }
    }

    public class Campaigns
    {
        public List<Success> success { get; set; }
        public List<Error> error { get; set; }
        public Campaigns()
        {
            success = new List<Success>();
            error = new List<Error>();
        }
    }

    public class Cause
    {
        public string location { get; set; }
        public string trigger { get; set; }
    }

    public class CurrencyError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public CurrencyError()
        {
            cause = new Cause();
        }
    }

    public class DateError
    {
        public string reason { get; set; }
        public Cause cause { get; set; }
        public string message { get; set; }
        public DateError()
        {
            cause = new Cause();
        }
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

    public class DynamicBiddingCampaign
    {
        public List<PlacementBiddingCampaign> placementBidding { get; set; }
        public string strategy { get; set; }
        public DynamicBiddingCampaign()
        {
            placementBidding = new List<PlacementBiddingCampaign>();
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
        public DateError dateError { get; set; }
        public BiddingError biddingError { get; set; }
        public DuplicateValueError duplicateValueError { get; set; }
        public RangeError rangeError { get; set; }
        public ParentEntityError parentEntityError { get; set; }
        public OtherError otherError { get; set; }
        public ThrottledError throttledError { get; set; }
        public EntityNotFoundError entityNotFoundError { get; set; }
        public MalformedValueError malformedValueError { get; set; }
        public BudgetError budgetError { get; set; }
        public CurrencyError currencyError { get; set; }
        public BillingError billingError { get; set; }
        public EntityQuotaError entityQuotaError { get; set; }
        public InternalServerError internalServerError { get; set; }
        public ErrorValue()
        {
            entityStateError = new EntityStateError();
            missingValueError = new MissingValueError();
            dateError = new DateError();
            biddingError = new BiddingError();
            duplicateValueError = new DuplicateValueError();
            rangeError = new RangeError();
            parentEntityError = new ParentEntityError();
            otherError = new OtherError();
            throttledError = new ThrottledError();
            entityNotFoundError = new EntityNotFoundError();
            malformedValueError = new MalformedValueError();
            budgetError = new BudgetError();
            currencyError = new CurrencyError();
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

    public class PlacementBiddingCampaign
    {
        public int percentage { get; set; }
        public string placement { get; set; }
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

    public class CampaignUpdateResponse
    {
        public Campaigns campaigns { get; set; }
        public CampaignUpdateResponse()
        {
            campaigns = new Campaigns();
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
        public string campaignId { get; set; }
        public int index { get; set; }
        public CampaignUpdateValues campaign { get; set; }
        public Success()
        {
            campaign = new CampaignUpdateValues();
        }
    }

    public class TagsCampaign
    {
        public string property1 { get; set; }
        public string property2 { get; set; }
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
