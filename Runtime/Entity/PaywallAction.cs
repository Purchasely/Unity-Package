namespace PurchaselyRuntime
{
	public class PaywallAction
	{
		public Parameters parameters;
		public Info info;
		public string action;

		public class Parameters
		{
			public string title;
			public string url;
			public Plan plan;
			public string presentation;
		}

		public class Info
		{
			public string contentId;
			public string presentationId;
			public string placementId;
			public string abTestId;
			public string abTestVariantId;
		}
	}
}