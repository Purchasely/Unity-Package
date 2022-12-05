using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	public class PresentationResultProxy : AndroidJavaProxy
	{
		private readonly Action<ProductViewResult, Plan> _onResult;

		internal PresentationResultProxy(Action<ProductViewResult, Plan> onResult) : base(
			"com.purchasely.unity.proxy.PresentationResultProxy")
		{
			_onResult = onResult;
		}

		public void onPresentationResult(int result, string planJson)
		{
			if (_onResult == null)
				return;

			AsyncCallbackHelper.Instance.Queue(
				() => _onResult((ProductViewResult) result, SerializationUtils.Deserialize<Plan>(planJson)));
		}
	}
}