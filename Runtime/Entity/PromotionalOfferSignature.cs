#if UNITY_IOS
using System;
#endif
using System.Collections.Generic;
#if UNITY_ANDROID
using UnityEngine;
#endif

namespace PurchaselyRuntime
{
    public class PromotionalOfferSignature
    {
        public string planVendorId;
        public string identifier;
        public string signature;
        public string keyIdentifier;
        public string nonce;
        public double timestamp;
    }
}