using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	public class PaywallInterceptorProxy : AndroidJavaProxy
	{
		private readonly Action<PaywallAction> _onAction;

		internal PaywallInterceptorProxy(Action<PaywallAction> onAction) : base(
			"com.purchasely.unity.proxy.PaywallInterceptorProxy")
		{
			_onAction = onAction;
		}

		public void onAction(string dataJson)
		{
			AsyncCallbackHelper.Instance.Queue(() =>
			{
				if (Debug.isDebugBuild)
					Debug.Log(dataJson);

				_onAction(SerializationUtils.Deserialize<PaywallAction>(dataJson));
			});
		}
	}
}