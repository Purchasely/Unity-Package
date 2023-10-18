#if UNITY_IOS
using System;
#endif
using System.Collections.Generic;
#if UNITY_ANDROID
using UnityEngine;
#endif

namespace PurchaselyRuntime
{
    public class PresentationPlan
    {
        public string planVendorId = null;
        public string offerId = null;
        public string storeProductId = null;
        public string basePlanId = null;
        
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