#if UNITY_IOS && !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PurchaselyRuntime
{
	public class PurchaselyIos : IPurchasely
	{
		public void Init(string apiKey, string userId, bool storekit1,
			int logLevel, int runningMode, Action<bool, string> onStartCompleted)
		{
			var startCallback = new Action<bool, string>((success, error) =>
			{
				AsyncCallbackHelper.Instance.Queue(() => { onStartCompleted(success, error); });
			});

			_purchaselyStart(apiKey, userId, logLevel, runningMode, storekit1,
				IosUtils.StartCallback, startCallback.GetPointer());
		}

		public void UserLogin(string userId, Action<bool> onCompleted)
		{
			var completeCallback = new Action<bool>(needRefresh =>
			{
				AsyncCallbackHelper.Instance.Queue(() => { onCompleted(needRefresh); });
			});

			_purchaselyUserLogin(userId, IosUtils.BoolCallback, completeCallback.GetPointer());
		}

		public void SetIsReadyToOpenDeeplink(bool ready)
		{
			_purchaselySetIsReadyToOpenDeeplink(ready);
		}

		public void PresentPresentationForPlacement(string placementId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId)
		{
			var resultCallback = new Action<int, string>((resultInt, planJson) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onResult((ProductViewResult)resultInt, SerializationUtils.Deserialize<Plan>(planJson));
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

			_purchaselyPresentPresentationForPlacement(placementId, contentId,
				IosUtils.BoolCallback, contentLoadCallback.GetPointer(),
				IosUtils.VoidCallback, closeButtonCallback.GetPointer(),
				IosUtils.PresentationResultCallback, resultCallback.GetPointer());
		}

		public void PresentPresentationWithId(string presentationId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded,
			Action onCloseButtonClicked, string contentId)
		{
			var resultCallback = new Action<int, string>((resultInt, planJson) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onResult((ProductViewResult)resultInt, SerializationUtils.Deserialize<Plan>(planJson));
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

			_purchaselyPresentPresentationWithId(presentationId, contentId,
				IosUtils.BoolCallback, contentLoadCallback.GetPointer(),
				IosUtils.VoidCallback, closeButtonCallback.GetPointer(),
				IosUtils.PresentationResultCallback, resultCallback.GetPointer());
		}

		public void PresentPresentationForProduct(string productId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked,
			string contentId, string presentationId)
		{
			var resultCallback = new Action<int, string>((resultInt, planJson) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onResult((ProductViewResult)resultInt, SerializationUtils.Deserialize<Plan>(planJson));
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

			_purchaselyPresentPresentationForProduct(productId, presentationId, contentId,
				IosUtils.BoolCallback, contentLoadCallback.GetPointer(),
				IosUtils.VoidCallback, closeButtonCallback.GetPointer(),
				IosUtils.PresentationResultCallback, resultCallback.GetPointer());
		}

		public void PresentPresentationForPlan(string planId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked,
			string contentId, string presentationId)
		{
			var resultCallback = new Action<int, string>((resultInt, planJson) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onResult((ProductViewResult)resultInt, SerializationUtils.Deserialize<Plan>(planJson));
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

			_purchaselyPresentPresentationForPlan(planId, presentationId, contentId,
				IosUtils.BoolCallback, contentLoadCallback.GetPointer(),
				IosUtils.VoidCallback, closeButtonCallback.GetPointer(),
				IosUtils.PresentationResultCallback, resultCallback.GetPointer());
		}

		public void SetPaywallActionInterceptor(Action<PaywallAction> onAction)
		{
			var actionCallback = new Action<string>(actionJson =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onAction(SerializationUtils.Deserialize<PaywallAction>(actionJson));
				});
			});

			_purchaselySetPaywallActionInterceptor(IosUtils.StringCallback, actionCallback.GetPointer());
		}

		public void ProcessPaywallAction(bool process)
		{
			_purchaselyProcessAction(process);
		}

		public void RestoreAllProducts(bool silent, Action<Plan> onSuccess, Action<string> onError)
		{
			var planCallback = new Action<string>(planJson =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onSuccess(SerializationUtils.Deserialize<Plan>(planJson));
				});
			});

			_purchaselyRestoreAllProducts(silent, IosUtils.StringCallback, planCallback.GetPointer(),
				IosUtils.StringCallback, onError.GetPointer());
		}

		public string GetAnonymousUserId()
		{
			return _purchaselyGetAnonymousUserId();
		}

		public void SetLanguage(string language)
		{
			_purchaselySetLanguage(language);
		}

		public void UserLogout()
		{
			_purchaselyUserLogout();
		}

		public void SetDefaultPresentationResultHandler(Action<ProductViewResult, Plan> onResult)
		{
			var resultCallback = new Action<int, string>((resultInt, planJson) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onResult((ProductViewResult)resultInt, SerializationUtils.Deserialize<Plan>(planJson));
				});
			});

			_purchaselySetDefaultPresentationResultHandler(IosUtils.PresentationResultCallback,
				resultCallback.GetPointer());
		}

		public void ProductWithIdentifier(string productId, Action<Product> onSuccess, Action<string> onError)
		{
			var productCallback = new Action<string>(productJson =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onSuccess(SerializationUtils.Deserialize<Product>(productJson));
				});
			});

			_purchaselyGetProduct(productId, IosUtils.StringCallback, productCallback.GetPointer(),
				IosUtils.StringCallback, onError.GetPointer());
		}

		public void PlanWithIdentifier(string planId, Action<Plan> onSuccess, Action<string> onError)
		{
			var planCallback = new Action<string>(planJson =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onSuccess(SerializationUtils.Deserialize<Plan>(planJson));
				});
			});

			_purchaselyGetPlan(planId, IosUtils.StringCallback, planCallback.GetPointer(),
				IosUtils.StringCallback, onError.GetPointer());
		}

		public void AllProducts(Action<List<Product>> onSuccess, Action<string> onError)
		{
			var productsCallback = new Action<string>(productsJson =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onSuccess(SerializationUtils.Deserialize<List<Product>>(productsJson));
				});
			});

			_purchaselyGetAllProducts(IosUtils.StringCallback, productsCallback.GetPointer(),
				IosUtils.StringCallback, onError.GetPointer());
		}

		public void Purchase(string planId,
			Action<Plan> onSuccess,
			Action<string> onError,
			string offerId,
			string contentId)
		{
			var planCallback = new Action<string>(planJson =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onSuccess(SerializationUtils.Deserialize<Plan>(planJson));
				});
			});

			_purchaselyPurchase(planId, offerId, IosUtils.StringCallback, planCallback.GetPointer(),
				IosUtils.StringCallback, onError.GetPointer());
		}

		public bool IsDeeplinkHandled(string url)
		{
			return _purchaselyIsDeeplinkHandled(url);
		}

		public void GetUserSubscriptions(Action<List<SubscriptionData>> onSuccess, Action<string> onError)
		{
			var subscriptionsCallback = new Action<string>(productsJson =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onSuccess(SerializationUtils.Deserialize<List<SubscriptionData>>(productsJson));
				});
			});

			_purchaselyGetUserSubscriptions(IosUtils.StringCallback, subscriptionsCallback.GetPointer(),
				IosUtils.StringCallback, onError.GetPointer());
		}

		public void PresentSubscriptions()
		{
			_purchaselyPresentSubscriptions();
		}

  	public void SetAttribute(int attribute, string value)
		{
			_purchaselySetAttribute(attribute, value);
		}

		public void SetUserAttribute(string key, string value)
		{
			_purchaselySetStringAttribute(key, value);
		}

		public void SetUserAttribute(string key, int value)
		{
			_purchaselySetIntAttribute(key, value);
		}

		public void SetUserAttribute(string key, float value)
		{
			_purchaselySetFloatAttribute(key, value);
		}

		public void SetUserAttribute(string key, bool value)
		{
			_purchaselySetBoolAttribute(key, value);
		}

		public void SetUserAttribute(string key, DateTime value)
		{
			_purchaselySetDateAttribute(key, value.ToIso8601Format());
		}

		public void ClearUserAttribute(string key)
		{
			_purchaselyClearAttribute(key);
		}

		public void ClearUserAttributes()
		{
			_purchaselyClearAttributes();
		}

		public string GetUserAttribute(string key)
		{
			return _purchaselyGetUserAttribute(key);
		}

		public void UserDidConsumeSubscriptionContent()
		{
			_purchaselyUserDidConsumeSubscriptionContent();
		}

		public bool IsAnonymous()
		{
			return _purchaselyIsAnonymous();
		}

		public void IsEligibleForIntroOffer(string planVendorId, Action<bool> onSuccess, Action<string> onError) {

			var eligibilityCallback = new Action<bool>(isEligible =>
			{
				AsyncCallbackHelper.Instance.Queue(() => { onSuccess(isEligible); });
			});

			_purchaselyIsEligibleForIntroOffer(planVendorId,
				IosUtils.BoolCallback, eligibilityCallback.GetPointer(),
				IosUtils.StringCallback, onError.GetPointer());
		}

		public void SignPromotionalOffer(string storeOfferId, string storeProductId, Action<PromotionalOfferSignature> onSuccess,
			Action<string> onError) {

			var signatureCallback = new Action<string, IntPtr>((json, pointer) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					var signature = SerializationUtils.Deserialize<PromotionalOfferSignature>(json);
					onSuccess(signature);
				});
			});

			_purchaselySignPromotionalOffer(storeOfferId, storeProductId,
				IosUtils.SignatureCallback, signatureCallback.GetPointer(),
				IosUtils.StringCallback, onError.GetPointer());
		}

	public void FetchPresentation(string presentationId, Action<Presentation> onSuccess, Action<string> onError,
			string contentId)
		{
			var presentationCallback = new Action<string, IntPtr>((json, pointer) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					var presentation = SerializationUtils.Deserialize<Presentation>(json);
					presentation.iosPresentation = pointer;
					presentation.presentationType =
						(PresentationType) Enum.Parse(typeof(PresentationType), presentation.type, true);
					onSuccess(presentation);
				});
			});

			_purchaselyFetchPresentation(presentationId, contentId, IosUtils.PresentationCallback,
				presentationCallback.GetPointer(),
				IosUtils.StringCallback, onError.GetPointer());
		}

		public void FetchPresentationForPlacement(string placementId, Action<Presentation> onSuccess,
			Action<string> onError, string contentId)
		{
			var presentationCallback = new Action<string, IntPtr>((json, pointer) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					var presentation = SerializationUtils.Deserialize<Presentation>(json);
					presentation.presentationType =
						(PresentationType) Enum.Parse(typeof(PresentationType), presentation.type, true);
					presentation.iosPresentation = pointer;
					onSuccess(presentation);
				});
			});

			_purchaselyFetchPresentationForPlacement(placementId, contentId, IosUtils.PresentationCallback,
				presentationCallback.GetPointer(),
				IosUtils.StringCallback, onError.GetPointer());
		}

		public void ClientPresentationOpened(Presentation presentation)
		{
			_purchaselyClientPresentationOpened(presentation.iosPresentation);
		}

		public void ClientPresentationClosed(Presentation presentation)
		{
			_purchaselyClientPresentationClosed(presentation.iosPresentation);
		}

		public void ClosePresentation()
		{
			_purchaselyClosePresentation();
		}

		public void HidePresentation()
		{
			_purchaselyHidePresentation();
		}

		public void ShowPresentation()
		{
			_purchaselyShowPresentation();
		}

		public void SetThemeMode(ThemeMode mode)
		{
			_purchaselySetThemeMode((int)mode);
		}

		public void PresentContentForPresentation(Presentation presentation, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded = null,
			Action onCloseButtonClicked = null)
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

			_purchaselyShowContentForPresentationObject(presentation.iosPresentation,
				IosUtils.BoolCallback, contentLoadCallback.GetPointer(),
				IosUtils.VoidCallback, closeButtonCallback.GetPointer(),
				IosUtils.PresentationResultCallback, resultCallback.GetPointer());
		}

		[DllImport("__Internal")]
		static extern void _purchaselyStart(string apiKey, string userId, int logLevel,
			int runningMode, bool storekit1, IosUtils.StartCallbackDelegate startCallback, IntPtr startCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyUserLogin(string userId, IosUtils.BoolCallbackDelegate onUserLogin,
			IntPtr onUserLoginPtr);

		[DllImport("__Internal")]
		static extern void _purchaselySetIsReadyToOpenDeeplink(bool ready);

		[DllImport("__Internal")]
		static extern void _purchaselyPresentPresentationWithId(string presentationId, string
				contentId, IosUtils.BoolCallbackDelegate loadCallback, IntPtr
				loadCallbackPtr, IosUtils.VoidCallbackDelegate closeCallback, IntPtr closeCallbackPtr,
			IosUtils.PresentationResultCallbackDelegate presentationResultCallback, IntPtr
				presentationResultCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyPresentPresentationForPlacement(string placementId, string contentId,
			IosUtils.BoolCallbackDelegate loadCallback, IntPtr loadCallbackPtr,
			IosUtils.VoidCallbackDelegate closeCallback, IntPtr closeCallbackPtr,
			IosUtils.PresentationResultCallbackDelegate presentationResultCallback,
			IntPtr presentationResultCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyPresentPresentationForPlan(string planId, string presentationId, string
				contentId, IosUtils.BoolCallbackDelegate loadCallback, IntPtr
				loadCallbackPtr, IosUtils.VoidCallbackDelegate closeCallback, IntPtr closeCallbackPtr,
			IosUtils.PresentationResultCallbackDelegate presentationResultCallback, IntPtr
				presentationResultCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyPresentPresentationForProduct(string productId, string presentationId, string
				contentId, IosUtils.BoolCallbackDelegate loadCallback, IntPtr
				loadCallbackPtr, IosUtils.VoidCallbackDelegate closeCallback, IntPtr closeCallbackPtr,
			IosUtils.PresentationResultCallbackDelegate presentationResultCallback, IntPtr
				presentationResultCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyPresentSubscriptions();

		[DllImport("__Internal")]
		static extern void _purchaselyPurchase(string planId, string offerId, IosUtils.StringCallbackDelegate successCallback, IntPtr
			successCallbackPtr, IosUtils.StringCallbackDelegate errorCallback, IntPtr errorCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyRestoreAllProducts(bool isSilent, IosUtils.StringCallbackDelegate successCallback,
			IntPtr successCallbackPtr, IosUtils.StringCallbackDelegate errorCallback, IntPtr errorCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyGetAllProducts(IosUtils.StringCallbackDelegate successCallback,
			IntPtr successCallbackPtr,
			IosUtils.StringCallbackDelegate errorCallback, IntPtr errorCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyGetProduct(string productId, IosUtils.StringCallbackDelegate successCallback,
			IntPtr
				successCallbackPtr, IosUtils.StringCallbackDelegate errorCallback, IntPtr errorCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyGetPlan(string planId, IosUtils.StringCallbackDelegate successCallback, IntPtr
			successCallbackPtr, IosUtils.StringCallbackDelegate errorCallback, IntPtr errorCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyGetUserSubscriptions(IosUtils.StringCallbackDelegate successCallback,
			IntPtr successCallbackPtr,
			IosUtils.StringCallbackDelegate errorCallback, IntPtr errorCallbackPtr);

		[DllImport("__Internal")]
		static extern bool _purchaselyIsDeeplinkHandled(string urlString);

		[DllImport("__Internal")]
		static extern void _purchaselySetLanguage(string language);

		[DllImport("__Internal")]
		static extern void _purchaselySetPaywallActionInterceptor(IosUtils.StringCallbackDelegate actionCallback,
			IntPtr actionCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyProcessAction(bool process);

		[DllImport("__Internal")]
		static extern void _purchaselyUserDidConsumeSubscriptionContent();

		[DllImport("__Internal")]
		static extern string _purchaselyGetAnonymousUserId();

		[DllImport("__Internal")]
		static extern void _purchaselyUserLogout();

		[DllImport("__Internal")]
		static extern void _purchaselySetDefaultPresentationResultHandler(
			IosUtils.PresentationResultCallbackDelegate presentationResultCallback,
			IntPtr presentationResultCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselySetAttribute(int attribute, string value);

		[DllImport("__Internal")]
		static extern void _purchaselySetStringAttribute(string key, string value);

		[DllImport("__Internal")]
		static extern void _purchaselySetBoolAttribute(string key, bool value);

		[DllImport("__Internal")]
		static extern void _purchaselySetIntAttribute(string key, int value);

		[DllImport("__Internal")]
		static extern void _purchaselySetFloatAttribute(string key, float value);

		[DllImport("__Internal")]
		static extern void _purchaselySetDateAttribute(string key, string dateString);

		[DllImport("__Internal")]
		static extern string _purchaselyGetUserAttribute(string key);

		[DllImport("__Internal")]
		static extern void _purchaselyClearAttribute(string key);

		[DllImport("__Internal")]
		static extern void _purchaselyClearAttributes();

		[DllImport("__Internal")]
		static extern void _purchaselyFetchPresentation(string presentationId, string contentId,
			IosUtils.PresentationCallbackDelegate successCallback, IntPtr successCallbackPtr,
			IosUtils.StringCallbackDelegate errorCallback, IntPtr errorCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyFetchPresentationForPlacement(string placementId, string contentId,
			IosUtils.PresentationCallbackDelegate successCallback, IntPtr successCallbackPtr,
			IosUtils.StringCallbackDelegate errorCallback, IntPtr errorCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyClientPresentationOpened(IntPtr presentationPointer);

		[DllImport("__Internal")]
		static extern void _purchaselyClientPresentationClosed(IntPtr presentationPointer);

		[DllImport("__Internal")]
		static extern void _purchaselyShowContentForPresentationObject(IntPtr presentationPointer,
			IosUtils.BoolCallbackDelegate loadCallback, IntPtr loadCallbackPtr,
			IosUtils.VoidCallbackDelegate closeCallback, IntPtr closeCallbackPtr,
			IosUtils.PresentationResultCallbackDelegate presentationResultCallback,
			IntPtr presentationResultCallbackPtr);

		[DllImport("__Internal")]
		static extern bool _purchaselyIsAnonymous();

		[DllImport("__Internal")]
		static extern void _purchaselyIsEligibleForIntroOffer(string planVendorId, IosUtils.BoolCallbackDelegate successCallback, IntPtr successCallbackPtr,
			IosUtils.StringCallbackDelegate errorCallback, IntPtr errorCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselySignPromotionalOffer(string storeOfferId, string storeProductId, IosUtils.SignatureCallbackDelegate successCallback, IntPtr successCallbackPtr,
			IosUtils.StringCallbackDelegate errorCallback, IntPtr errorCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyClosePresentation();

		[DllImport("__Internal")]
		static extern void _purchaselyHidePresentation();

		[DllImport("__Internal")]
		static extern void _purchaselyShowPresentation();

		[DllImport("__Internal")]
		static extern void _purchaselySetThemeMode(int mode);
	}
}

#endif
