#if UNITY_IOS
using System;
#endif
using System.Collections.Generic;
#if UNITY_ANDROID
using UnityEngine;
#endif

using System;

namespace PurchaselyRuntime
{
    public class PresentationPlan
    {
        public string planVendorId;
        public string offerId;
        public string storeProductId;
        public string basePlanId;

        public override string ToString()
        {
            return $"PresentationPlan - \n" +
                   $"  PlanVendorId: {planVendorId}\n" +
                   $"  OfferId: {offerId}\n" +
                   $"  StoreProductId: {storeProductId}\n" +
                   $"  BasePlanId: {basePlanId}";
        }
    }
}