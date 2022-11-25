using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	internal class EventProxy : AndroidJavaProxy
	{
		private readonly Action<PurchaselyEvent> _onEvent;

		internal EventProxy(Action<PurchaselyEvent> onEvent) : base("com.purchasely.unity.proxy.EventProxy")
		{
			_onEvent = onEvent;
		}

		public void onEventReceived(string name, string propertiesJson)
		{
			AsyncCallbackHelper.Instance.Queue(() => _onEvent(new PurchaselyEvent(name, propertiesJson)));
		}
	}
}
