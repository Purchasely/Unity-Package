namespace PurchaselyRuntime
{
	public class SubscriptionData
	{
		public Subscription subscription;
		public Plan plan;
		public Product product;

		public class Subscription
		{
			public string cancelledAt;
			public string contentId;
			public string environment;
			public string id;
			public bool isFamilyShared;
			public string nextRenewalAt;
			public string offerIdentifier;
			public string offerType;
			public string originalPurchasedAt;
			public string planId;
			public string purchaseToken;
			public string purchasedAt;
			public string storeCountry;
			public string storeType;
			public string subscriptionStatus;
		}
	}
}