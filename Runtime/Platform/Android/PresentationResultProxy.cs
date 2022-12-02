using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	public class PresentationResultProxy : AndroidJavaProxy
	{
		private readonly Action<ProductViewResult, PurchaselyPlan> _onResult;

		internal PresentationResultProxy(Action<ProductViewResult, PurchaselyPlan> onResult) : base(
			"com.purchasely.unity.proxy.PresentationResultProxy")
		{
			_onResult = onResult;
		}

		public void onPresentationResult(int result, string planJson)
		{
			if (_onResult == null)
				return;

			AsyncCallbackHelper.Instance.Queue(
				() => _onResult((ProductViewResult) result, new PurchaselyPlan(planJson)));
		}
	}
}