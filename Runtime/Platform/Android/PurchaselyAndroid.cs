#if UNITY_ANDROID && !UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace PurchaselyRuntime
{
	internal class PurchaselyAndroid : IPurchasely
	{
		private AndroidJavaObject _javaBridge;

		public void Init(string apiKey, string userId, bool readyToPurchase, int logLevel,
			int runningMode, Action<bool, string> onStartCompleted, Action<Event> onEventReceived)
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

		public void PresentPresentationForPlacement(string placementId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId)
		{
			_javaBridge?.Call("showContentForPlacement", AndroidUtils.Activity, placementId,
				new PlacementContentProxy(onContentLoaded, onCloseButtonClicked, onResult), contentId);
		}

		public void PresentContentForPresentation(string presentationId,
			Action<ProductViewResult, Plan> onResult, Action<bool> onContentLoaded,
			Action onCloseButtonClicked, string contentId)
		{
			_javaBridge?.Call("showContentForPresentation", AndroidUtils.Activity, presentationId,
				new PlacementContentProxy(onContentLoaded, onCloseButtonClicked, onResult), contentId);
		}

		public void PresentPresentationForProduct(string productId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId, string presentationId)
		{
			_javaBridge?.Call("showContentForProduct", AndroidUtils.Activity, productId,
				new PlacementContentProxy(onContentLoaded, onCloseButtonClicked, onResult), contentId, presentationId);
		}

		public void PresentPresentationForPlan(string planId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId, string presentationId)
		{
			_javaBridge?.Call("showContentForPlan", AndroidUtils.Activity, planId,
				new PlacementContentProxy(onContentLoaded, onCloseButtonClicked, onResult), contentId, presentationId);
		}

		public void SetPaywallActionInterceptor(Action<PaywallAction> onAction)
		{
			_javaBridge?.Call("setPaywallActionInterceptor", new PaywallInterceptorProxy(onAction));
		}

		public void ProcessPaywallAction(bool process)
		{
			_javaBridge?.Call("processPaywallAction", AndroidUtils.Activity, process);
		}

		public void RestoreAllProducts(bool silent, Action<Plan> onSuccess, Action<string> onError)
		{
			var successAction = new Action<string>(json => { onSuccess(SerializationUtils.Deserialize<Plan>(json)); });

			_javaBridge?.Call("restoreAllProducts", silent, new JsonErrorProxy(successAction, onError));
		}

		public string GetAnonymousUserId()
		{
			return _javaBridge?.Call<string>("getAnonymousUserId") ?? string.Empty;
		}

		public void SetLanguage(string language)
		{
			_javaBridge?.Call("setLanguage", language);
		}

		public void UserLogout()
		{
			_javaBridge?.Call("userLogout");
		}

		public void SetDefaultPresentationResultHandler(Action<ProductViewResult, Plan> onResult)
		{
			_javaBridge?.Call("setDefaultPresentationResultHandler", new PresentationResultProxy(onResult));
		}

		public void ProductWithIdentifier(string productId, Action<Product> onSuccess, Action<string> onError)
		{
			var successAction =
				new Action<string>(json => { onSuccess(SerializationUtils.Deserialize<Product>(json)); });

			_javaBridge?.Call("productWithIdentifier", productId, new JsonErrorProxy(successAction, onError));
		}

		public void PlanWithIdentifier(string planId, Action<Plan> onSuccess, Action<string> onError)
		{
			var successAction = new Action<string>(json => { onSuccess(SerializationUtils.Deserialize<Plan>(json)); });

			_javaBridge?.Call("planWithIdentifier", planId, new JsonErrorProxy(successAction, onError));
		}

		public void AllProducts(Action<List<Product>> onSuccess, Action<string> onError)
		{
			var successAction = new Action<string>(json =>
			{
				onSuccess(SerializationUtils.Deserialize<List<Product>>(json));
			});

			_javaBridge?.Call("allProducts", new JsonErrorProxy(successAction, onError));
		}

		public void PurchaseWithPlanId(string planId, Action<Plan> onSuccess, Action<string> onError, string contentId)
		{
			var successAction = new Action<string>(json => { onSuccess(SerializationUtils.Deserialize<Plan>(json)); });

			_javaBridge?.Call("purchaseWithPlanId", AndroidUtils.Activity, planId, contentId,
				new JsonErrorProxy(successAction, onError));
		}

		public bool HandleDeepLinkUrl(string url)
		{
			return _javaBridge?.Call<bool>("handleDeepLinkUrl", url) ?? false;
		}

		public void GetUserSubscriptions(Action<List<SubscriptionData>> onSuccess, Action<string> onError)
		{
			var successAction = new Action<string>(json =>
			{
				onSuccess(SerializationUtils.Deserialize<List<SubscriptionData>>(json));
			});

			_javaBridge?.Call("getUserSubscriptions", new JsonErrorProxy(successAction, onError));
		}

		public void PresentSubscriptions()
		{
			_javaBridge?.Call("presentSubscriptions", AndroidUtils.Activity);
		}

		public void SetUserAttribute(string key, string value)
		{
			_javaBridge?.Call("setUserAttribute", key, value);
		}

		public void SetUserAttribute(string key, int value)
		{
			_javaBridge?.Call("setUserAttribute", key, value);
		}

		public void SetUserAttribute(string key, float value)
		{
			_javaBridge?.Call("setUserAttribute", key, value);
		}

		public void SetUserAttribute(string key, bool value)
		{
			_javaBridge?.Call("setUserAttribute", key, value);
		}

		public void SetUserAttribute(string key, DateTime value)
		{
			_javaBridge?.Call("setUserAttributeWithDate", key, value.ToIso8601Format());
		}

		public void ClearUserAttribute(string key)
		{
			_javaBridge?.Call("clearUserAttribute", key);
		}

		public void ClearUserAttributes()
		{
			_javaBridge?.Call("clearUserAttributes");
		}

		public string GetUserAttribute(string key)
		{
			return _javaBridge?.Call<string>("getUserAttribute", key) ?? string.Empty;
		}

		public void UserDidConsumeSubscriptionContent()
		{
			_javaBridge?.Call("userDidConsumeSubscriptionContent");
		}

		public void FetchPresentation(string presentationId, Action<Presentation> onSuccess, Action<string> onError,
			string contentId)
		{
			_javaBridge?.Call("fetchPresentation", AndroidUtils.Activity, presentationId, contentId, 
				new FetchPresentationProxy(onSuccess, onError));
		}

		public void FetchPresentationForPlacement(string placementId, Action<Presentation> onSuccess, 
			Action<string> onError, string contentId)
		{
			_javaBridge?.Call("fetchPresentationForPlacement", AndroidUtils.Activity, placementId, 
				contentId, new FetchPresentationProxy(onSuccess, onError));
		}

		public void ClientPresentationOpened(Presentation presentation)
		{
			_javaBridge?.Call("clientPresentationOpened", presentation.presentationAjo);
		}

		public void ClientPresentationClosed(Presentation presentation)
		{
			_javaBridge?.Call("clientPresentationClosed", presentation.presentationAjo);
		}

		public void PresentContentForPresentation(Presentation presentation, Action<ProductViewResult, Plan> onResult, 
			Action<bool> onContentLoaded = null, Action onCloseButtonClicked = null)
		{
			_javaBridge?.Call("showContentForPresentation", AndroidUtils.Activity, 
				presentation.presentationAjo, new PlacementContentProxy(onContentLoaded, onCloseButtonClicked, onResult));
		}

        public void PresentPresentationWithId(string presentationId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId)
        {
	        _javaBridge?.Call("showContentForPresentation", AndroidUtils.Activity, presentationId,
		        new PlacementContentProxy(onContentLoaded, onCloseButtonClicked, onResult), contentId);
        }

        public void ShowPresentation() {
            _javaBridge?.Call("showPresentation");
        }

        public void HidePresentation() {
            _javaBridge?.Call("hidePresentation");
        }

        public void ClosePresentation() {
            _javaBridge?.Call("closePresentation");
        }
    }
}

#endif