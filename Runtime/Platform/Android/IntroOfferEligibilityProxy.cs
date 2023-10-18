using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	public class IntroOfferEligibilityProxy : AndroidJavaProxy
	{
		private readonly Action<bool> _onSuccess;
		private readonly Action<string> _onError;

		internal IntroOfferEligibilityProxy(Action<bool> onSuccess, Action<string> onError) : base(
			"com.purchasely.unity.proxy.IntroOfferEligibilityProxy")
		{
			_onSuccess = onSuccess;
			_onError = onError;
		}

		public void onSuccess(bool isEligible)
        {
            AsyncCallbackHelper.Instance.Queue(() => _onSuccess(isEligible));
        }

		public void onError(string error)
		{
			AsyncCallbackHelper.Instance.Queue(() => _onError(error));
		}
	}
}