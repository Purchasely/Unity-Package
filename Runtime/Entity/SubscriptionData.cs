namespace PurchaselyRuntime
{
	public class SubscriptionData
	{
		public Subscription subscription;
		public Plan plan;
		public Product product;

		public class Subscription
		{
			public string cancelled_at;
			public string content_id;
			public string environment;
			public string id;
			public bool is_family_shared;
			public string next_renewal_at;
			public string offer_identifier;
			public string offer_type;
			public string original_purchased_at;
			public string plan_id;
			public string purchase_token;
			public string purchased_at;
			public string store_country;
			public string store_type;
			public string subscription_status;
		}
	}
}