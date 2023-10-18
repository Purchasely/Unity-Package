using System;

namespace PurchaselyRuntime
{
	public class SubscriptionOffer
	{
		public string subscriptionId;
		public string? basePlanId;
		public string? offerToken;
		public string? offerId;

		public override string ToString()
        {
            return $"subscriptionId: {subscriptionId}, basePlanId: {basePlanId}, offerToken: {offerToken}, offerId: {offerId}";
        }
	}
}