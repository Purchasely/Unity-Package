#if UNITY_IOS && !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PurchaselyRuntime
{
	public class PurchaselyIos : IPurchasely
	{
		public void Init(string apiKey, string userId, bool readyToPurchase, int logLevel,
			int runningMode, Action<bool, string> onStartCompleted, Action<Event> onEventReceived)
		{
			var startCallback = new Action<bool, string>((success, error) =>
			{
				AsyncCallbackHelper.Instance.Queue(() => { onStartCompleted(success, error); });
			});

			var eventCallback = new Action<string>((propertiesJson) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onEventReceived(SerializationUtils.Deserialize<Event>(propertiesJson));
				});
			});

			_purchaselyStart(apiKey, userId, readyToPurchase, logLevel, runningMode,
				IosUtils.StartCallback, startCallback.GetPointer(),
				IosUtils.EventCallback, eventCallback.GetPointer());
		}

		public void UserLogin(string userId, Action<bool> onCompleted)
		{
			var completeCallback = new Action<bool>(needRefresh =>
			{
				AsyncCallbackHelper.Instance.Queue(() => { onCompleted(needRefresh); });
			});

			_purchaselyUserLogin(userId, IosUtils.BoolCallback, completeCallback.GetPointer());
		}

		public void SetIsReadyToPurchase(bool ready)
		{
			_purchaselySetIsReadyToPurchase(ready);
		}

		public void PresentContentForPlacement(string placementId, Action<ProductViewResult, Plan> onResult, 
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId)
		{
			var resultCallback = new Action<int, string>((resultInt, planJson) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onResult((ProductViewResult) resultInt, SerializationUtils.Deserialize<Plan>(planJson));
				});
			});

			var contentLoadCallback = new Action<bool>(isLoaded =>
			{
				if (onContentLoaded != null)
					AsyncCallbackHelper.Instance.Queue(() => { onContentLoaded(isLoaded); });
			});

			var closeButtonCallback = new Action(() =>
			{
				if (onCloseButtonClicked != null)
					AsyncCallbackHelper.Instance.Queue(onCloseButtonClicked);
			});

			if (contentId == null)
				contentId = string.Empty;

			_purchaselyShowContentForPlacement(placementId, contentId,
				IosUtils.BoolCallback, contentLoadCallback.GetPointer(),
				IosUtils.VoidCallback, closeButtonCallback.GetPointer(),
				IosUtils.PresentationResultCallback, resultCallback.GetPointer());
		}

		public void PresentContentForPresentation(string presentationId, Action<ProductViewResult, Plan> onResult, Action<bool> onContentLoaded,
			Action onCloseButtonClicked, string contentId)
		{
			throw new NotImplementedException();
		}

		public void PresentContentForProduct(string productId, Action<ProductViewResult, Plan> onResult, Action<bool> onContentLoaded, Action onCloseButtonClicked,
			string contentId, string presentationId)
		{
			throw new NotImplementedException();
		}

		public void PresentContentForPlan(string planId, Action<ProductViewResult, Plan> onResult, Action<bool> onContentLoaded, Action onCloseButtonClicked,
			string contentId, string presentationId)
		{
			throw new NotImplementedException();
		}

		public void SetPaywallActionInterceptor(Action<PaywallAction> onAction)
		{
			throw new NotImplementedException();
		}

		public void ProcessPaywallAction(bool process)
		{
			throw new NotImplementedException();
		}

		public void RestoreAllProducts(bool silent, Action<Plan> onSuccess, Action<string> onError)
		{
			throw new NotImplementedException();
		}

		public string GetAnonymousUserId()
		{
			throw new NotImplementedException();
		}

		public void SetLanguage(string language)
		{
			throw new NotImplementedException();
		}

		public void UserLogout()
		{
			throw new NotImplementedException();
		}

		public void SetDefaultPresentationResultHandler(Action<ProductViewResult, Plan> onResult)
		{
			throw new NotImplementedException();
		}

		public void ProductWithIdentifier(string productId, Action<Product> onSuccess, Action<string> onError)
		{
			throw new NotImplementedException();
		}

		public void PlanWithIdentifier(string planId, Action<Plan> onSuccess, Action<string> onError)
		{
			throw new NotImplementedException();
		}

		public void AllProducts(Action<List<Product>> onSuccess, Action<string> onError)
		{
			throw new NotImplementedException();
		}

		public void PurchaseWithPlanId(string planId, Action<Plan> onSuccess, Action<string> onError, string contentId)
		{
			throw new NotImplementedException();
		}

		public void HandleDeepLinkUrl(string url)
		{
			throw new NotImplementedException();
		}

		public void GetUserSubscriptions(Action<SubscriptionData> onSuccess, Action<string> onError)
		{
			throw new NotImplementedException();
		}

		public void PresentSubscriptions()
		{
			throw new NotImplementedException();
		}

		public void SetUserAttribute(string key, string value)
		{
			throw new NotImplementedException();
		}

		public void SetUserAttribute(string key, int value)
		{
			throw new NotImplementedException();
		}

		public void SetUserAttribute(string key, float value)
		{
			throw new NotImplementedException();
		}

		public void SetUserAttribute(string key, bool value)
		{
			throw new NotImplementedException();
		}

		public void SetUserAttribute(string key, DateTime value)
		{
			throw new NotImplementedException();
		}

		public void ClearUserAttribute(string key)
		{
			throw new NotImplementedException();
		}

		public void ClearUserAttributes()
		{
			throw new NotImplementedException();
		}

		public string GetUserAttribute(string key)
		{
			throw new NotImplementedException();
		}

		public void UserDidConsumeSubscriptionContent()
		{
			throw new NotImplementedException();
		}

		[DllImport("__Internal")]
		static extern void _purchaselyStart(string apiKey, string userId, bool readyToPurchase, int logLevel,
			int runningMode, IosUtils.StartCallbackDelegate startCallback, IntPtr startCallbackPtr,
			IosUtils.EventCallbackDelegate eventCallback, IntPtr eventCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyUserLogin(string userId, IosUtils.BoolCallbackDelegate onUserLogin,
			IntPtr onUserLoginPtr);

		[DllImport("__Internal")]
		static extern void _purchaselySetIsReadyToPurchase(bool ready);

		[DllImport("__Internal")]
		static extern void _purchaselyShowContentForPlacement(string placementId, string contentId,
			IosUtils.BoolCallbackDelegate loadCallback, IntPtr loadCallbackPtr,
			IosUtils.VoidCallbackDelegate closeCallback, IntPtr closeCallbackPtr,
			IosUtils.PresentationResultCallbackDelegate presentationResultCallback,
			IntPtr presentationResultCallbackPtr);
	}
}

#endif