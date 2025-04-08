using AdTool.AzSponsoredProducts.BusinessLogic.ProductManagement;
using AdTool.Entities.AzSp.ProductManagement;
using AdTool.Entities.Payments;
using AdTool.PaymentProcessor.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdToolApi.Controllers.Payment
{
    [Route("api/Payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpPost]
        [Route("CreateSubscription")] //api/Payment/CreateSubscription
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<SimplePaymentResponse>> CreateSubscription([FromBody] CreateSubscriptionRequestObject subscriptionRequest)
        {
            StripeCreateSubscriptionLogic createSubscriptionLogic = new StripeCreateSubscriptionLogic();
            var result = await createSubscriptionLogic.CreateSubscription(subscriptionRequest);
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("CancelSubscription")] //api/Payment/CancelSubscription
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]

        public async Task<ActionResult<SimplePaymentResponse>> CancelSubscription([FromBody] CancelSubscriptionRequestObject subscriptionRequest)
        {
            StripeCancelSubscriptionLogic cancel = new StripeCancelSubscriptionLogic();
            var result = await cancel.CancelSubscription(subscriptionRequest);
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("UpdateCreditCard")] //api/Payment/UpdateCreditCard
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<SimplePaymentResponse>> UpdateCreditCard([FromBody] CreateSubscriptionRequestObject subscriptionRequest)
        {
            StripeUpdateCCSubscriptionLogic paymentClass = new StripeUpdateCCSubscriptionLogic();
            var result = await paymentClass.UpdateCreditCard(subscriptionRequest);
            return result != null ? Ok(result) : NoContent();
        }

        [HttpPost]
        [Route("ChangePaymentPlan")] //api/Payment/ChangePaymentPlan
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<SimplePaymentResponse>> ChangePaymentPlan([FromBody] ChangePaymentPlanObject subscriptionRequest)
        {
            StripeChangePaymentPlanLogic paymentClass = new StripeChangePaymentPlanLogic();
            var result = await paymentClass.ChangePaymentPlan(subscriptionRequest);
            return result != null ? Ok(result) : NoContent();
        }
    }
}
