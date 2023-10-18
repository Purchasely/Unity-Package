﻿using System.Collections.Generic;

namespace PurchaselyRuntime
{
	public class Plan
	{
		public string id;
		public bool isVisible;
		public bool hasIntroductoryPrice;
		public bool isEligibleForIntroOffer;
		public bool hasFreeTrial;
		public int level;
		public float amount;
		public float introAmount;
		public string name;
		public string storeProductId;
		public string type;
		public string vendorId;
		public string localizedAmount;
		public string introPrice;
		public string period;
		public string productId;
		public string currencySymbol;
		public string currencyCode;
		public string introPeriod;
		public string introDuration;
		public string price;
		public List<PromoOffer> offers;
	}
}