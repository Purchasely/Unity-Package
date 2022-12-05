using System.Collections.Generic;

namespace PurchaselyRuntime
{
	public class Product
	{
		public string id;
		public string name;
		public string vendorId;
		public List<Plan> plans;
		public Conditions conditions;
		public Image image;

		public class Conditions
		{
			public string terms;
		}
		
		public class Image
		{
			public string key;
			public string url;
		}
	}
}