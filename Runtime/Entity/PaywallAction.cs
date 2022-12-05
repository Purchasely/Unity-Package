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
			public string content_id;
			public string presentation_id;
			public string placement_id;
			public string ab_test_id;
			public string ab_test_variant_id;
		}
	}
}