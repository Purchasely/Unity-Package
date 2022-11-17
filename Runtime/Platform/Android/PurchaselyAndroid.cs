using System;
using UnityEngine;

namespace Purchasely
{
	public class PurchaselyAndroid : IPurchasely
	{
		private AndroidJavaObject _javaBridge;

		public void Init(string apiKey, string userId, bool readyToPurchase, int storeFlags, int logLevel,
			int runningMode, Action<bool, string> onStartCompleted, Action<PurchaselyEvent> onEventReceived)
		{
			_javaBridge = new AndroidJavaObject("com.purchasely.unity.PurchaselyBridge",
				AndroidUtils.Activity,
				apiKey,
				userId,
				readyToPurchase,
				storeFlags,
				logLevel,
				runningMode,
				new StartProxy(onStartCompleted),
				new EventProxy(onEventReceived));
		}
	}
}