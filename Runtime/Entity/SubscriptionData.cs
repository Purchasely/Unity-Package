namespace PurchaselyRuntime
{
	public class SubscriptionData
	{
		public Plan plan;
		public Product product;
		public string contentId;
		public string environment;
		public string id;
		public bool isFamilyShared;
		public string offerIdentifier;
		public SubscriptionOfferType offerType;
		public string originalPurchasedAt;
		public string purchaseToken;
		public string purchasedDate;
		public string nextRenewalDate;
		public string cancelledDate;
		public string storeCountry;
		public StoreType storeType;
		public SubscriptionStatus status;
	}
}