using System;

namespace PurchaselyRuntime
{
	public class PromoOffer
	{
		public string vendorId;
		public string storeOfferId;

		public override string ToString()
        {
            return $"VendorId: {vendorId}, StoreOfferId: {storeOfferId}";
        }
	}
}