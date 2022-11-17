using System;
using Purchasely;
using UnityEngine;

internal class EventProxy : AndroidJavaProxy
{
	private readonly Action<PurchaselyEvent> _onEvent;

	internal EventProxy(Action<PurchaselyEvent> onEvent) : base("com.purchasely.unity.proxy.EventProxy")
	{
		_onEvent = onEvent;
	}

	public void onEventReceived(string id, string name, string propertiesJson)
	{
		AsyncCallbackHelper.Instance.Queue(() => _onEvent(new PurchaselyEvent(id, name, propertiesJson)));
	}
}