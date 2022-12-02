#if UNITY_ANDROID && !UNITY_EDITOR

using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	internal class PurchaselyAndroid : IPurchasely
	{
		private AndroidJavaObject _javaBridge;

		public void Init(string apiKey, string userId, bool readyToPurchase, int logLevel,
			int runningMode, Action<bool, string> onStartCompleted, Action<PurchaselyEvent> onEventReceived)
		{
			_javaBridge = new AndroidJavaObject("com.purchasely.unity.PurchaselyBridge",
				AndroidUtils.Activity,
				apiKey,
				userId,
				readyToPurchase,
				(int) Store.Google,
				logLevel,
				runningMode,
				new StartProxy(onStartCompleted),
				new EventProxy(onEventReceived));
		}

		public void UserLogin(string userId, Action<bool> onCompleted)
		{
			_javaBridge?.Call("userLogin", userId, new UserLoginProxy(onCompleted));
		}

		public void SetIsReadyToPurchase(bool ready)
		{
			_javaBridge?.Call("setIsReadyToPurchase", ready);
		}

		public void PresentContentForPlacement(string placementId, Action<ProductViewResult, PurchaselyPlan> onResult, 
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId)
		{
			_javaBridge?.Call("showContentForPlacement", AndroidUtils.Activity, placementId, 
				new PlacementContentProxy(onContentLoaded, onCloseButtonClicked, onResult), contentId);
		}
	}
}

#endif