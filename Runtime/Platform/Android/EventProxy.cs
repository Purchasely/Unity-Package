using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	internal class EventProxy : AndroidJavaProxy
	{
		private readonly Action<Event> _onEvent;

		internal EventProxy(Action<Event> onEvent) : base("com.purchasely.unity.proxy.EventProxy")
		{
			_onEvent = onEvent;
		}

		public void onEventReceived(string eventJson)
		{
			AsyncCallbackHelper.Instance.Queue(() =>
			{
				if (Debug.isDebugBuild)
					Debug.Log(eventJson);

				_onEvent(SerializationUtils.Deserialize<Event>(eventJson));
			});
		}
	}
}