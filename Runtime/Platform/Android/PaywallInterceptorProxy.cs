using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	public class PaywallInterceptorProxy : AndroidJavaProxy
	{
		private readonly Action<string> _onAction;

		internal PaywallInterceptorProxy(Action<string> onAction) : base(
			"com.purchasely.unity.proxy.PaywallInterceptorProxy")
		{
			_onAction = onAction;
		}

		public void onAction(string dataJson)
		{
			AsyncCallbackHelper.Instance.Queue(() => _onAction(dataJson));
		}
	}
}