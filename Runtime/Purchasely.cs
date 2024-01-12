using System;
using System.Collections.Generic;
using UnityEngine;

namespace PurchaselyRuntime
{
	/// <summary>
	/// Main entry point for communication with Purchasely SDK.
	/// </summary>
	public class Purchasely
	{
		public const string SettingsPath = "Purchasely/Settings";
		public const string SettingsFullPath = "Assets/Resources/" + SettingsPath + ".asset";

#pragma warning disable 649
		private readonly IPurchasely _implementation;
#pragma warning restore 649

		/// <summary>
		/// Create Purchasely object, through which all of the SDK interactions are completed.
		/// </summary>
		/// <param name="userId"> User ID. Pass empty string if you want the SDK to be bound to the device instead of user. </param>
		/// <param name="logLevel"> Log level of the SDK. </param>
		/// <param name="runningMode"> Allows you to use Purchasely with another In-App purchase system to prepare a migration. More details here: https://docs.purchasely.com/quick-start-1/sdk-configuration.</param>
		/// <param name="onStartCompleted"> Callback received with the result of the SDK initialization. Boolean parameter represents the success status with an optional error.</param>
		/// <exception cref="ArgumentException"> Is thrown if the SDK is not configured in the Editor. In the Unity Editor go to Window->Purchasely, then provide your API key and other required data.</exception>
		public Purchasely(string userId = null, bool storekit1 = false, LogLevel logLevel = LogLevel.Debug, RunningMode runningMode = RunningMode.Full, Action<bool, string> onStartCompleted = null)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			_implementation = new PurchaselyAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
			_implementation = new PurchaselyIos();
#endif
			var settings = Resources.Load<Settings>(SettingsPath);
			if (settings == null)
				throw new ArgumentException(
					$"Purchasely settings were not found. Please, make sure that asset \'{SettingsFullPath}\' exists. In the Unity Editor go to Window->Purchasely, then provide your API key and other required data.");

			if (_implementation == null)
			{
				onStartCompleted(false, "This platform is not supported.");
				return;
			}

			_implementation.Init(settings.ApiKey,
				userId,
				storekit1,
				(int) logLevel,
				(int) runningMode,
				onStartCompleted);
		}

		/// <summary>
		/// Once your user is logged in and you can send us a userId, please do it otherwise the purchase will be tied to the device and your user won't be able to enjoy from another device.
		/// Setting it will allow you to tie a purchase to a user to use it on other devices.
		/// This ID will be passed to the Web-hook so that your backend can identify the user and unlock the access.
		/// </summary>
		/// <param name="userId"> User ID to pass to the SDK. </param>
		/// <param name="onCompleted"> If our backend made a migration of user purchases and notified your backend, we will set the refresh variable in the callback to true. </param>
		public void UserLogin(string userId, Action<bool> onCompleted)
		{
			_implementation?.UserLogin(userId, onCompleted);
		}

		/// <summary>
		/// When your app is ready, call the following method and the SDK will handle the continuation of whatever was in progress (purchase, push message, …).
		/// This is mandatory to be able to handle Promoted In-App Purchases and Deep links automations.
		///
		/// Call this if you have previously passed `readyToPurchase = false` in constructor.
		/// </summary>
		/// <param name="ready"> Whether the application is ready to present the paywall and make purchases. </param>
		public void SetIsReadyToOpenDeeplink(bool ready)
		{
			_implementation?.SetIsReadyToOpenDeeplink(ready);
		}

		/// <summary>
		/// Show presentation for a specific placement.
		/// </summary>
		/// <param name="placementId"> Placement to show. </param>
		/// <param name="onResult"> Callback to be invoked after user action. </param>
		/// <param name="onContentLoaded"> Optional: callback to be invoked when the content is loaded. </param>
		/// <param name="onCloseButtonClicked"> Optional: callback to be invoked when the user taps the close button. </param>
		/// <param name="contentId"> Optional: content ID. </param>
		public void PresentPresentationForPlacement(string placementId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded = null, Action onCloseButtonClicked = null, string contentId = "")
		{
			if (string.IsNullOrEmpty(contentId))
				contentId = string.Empty;

			if (_implementation == null)
			{
				onResult(ProductViewResult.Cancelled, null);
				return;
			}

			_implementation.PresentPresentationForPlacement(placementId, onResult, onContentLoaded, onCloseButtonClicked,
				contentId);
		}

		/// <summary>
		/// Show content for a specific presentation.
		/// </summary>
		/// <param name="presentationId"> Presentation to show. </param>
		/// <param name="onResult"> Callback to be invoked after user action. </param>
		/// <param name="onContentLoaded"> Optional: callback to be invoked when the content is loaded. </param>
		/// <param name="onCloseButtonClicked"> Optional: callback to be invoked when the user taps the close button. </param>
		/// <param name="contentId"> Optional: content ID. </param>
		public void PresentPresentationWithId(string presentationId,
			Action<ProductViewResult, Plan> onResult, Action<bool> onContentLoaded = null,
			Action onCloseButtonClicked = null, string contentId = "")
		{
			if (string.IsNullOrEmpty(contentId))
				contentId = string.Empty;

			if (_implementation == null)
			{
				onResult(ProductViewResult.Cancelled, null);
				return;
			}

			_implementation.PresentPresentationWithId(presentationId, onResult, onContentLoaded,
				onCloseButtonClicked, contentId);
		}

		/// <summary>
		/// Show presentation for a specific product.
		/// </summary>
		/// <param name="productId"> Product to show. </param>
		/// <param name="onResult"> Callback to be invoked after user action. </param>
		/// <param name="onContentLoaded"> Optional: callback to be invoked when the content is loaded. </param>
		/// <param name="onCloseButtonClicked"> Optional: callback to be invoked when the user taps the close button. </param>
		/// <param name="contentId"> Optional: content ID. </param>
		/// <param name="presentationId"> Optional: presentation ID. </param>
		public void PresentPresentationForProduct(string productId,
			Action<ProductViewResult, Plan> onResult, Action<bool> onContentLoaded = null,
			Action onCloseButtonClicked = null, string contentId = "", string presentationId = "")
		{
			if (string.IsNullOrEmpty(contentId))
				contentId = string.Empty;

			if (_implementation == null)
			{
				onResult(ProductViewResult.Cancelled, null);
				return;
			}

			_implementation.PresentPresentationForProduct(productId, onResult, onContentLoaded,
				onCloseButtonClicked, contentId, presentationId);
		}

		/// <summary>
		/// Show presentation for a specific plan.
		/// </summary>
		/// <param name="planId"> Plan to show. </param>
		/// <param name="onResult"> Callback to be invoked after user action. </param>
		/// <param name="onContentLoaded"> Optional: callback to be invoked when the content is loaded. </param>
		/// <param name="onCloseButtonClicked"> Optional: callback to be invoked when the user taps the close button. </param>
		/// <param name="contentId"> Optional: content ID. </param>
		/// <param name="presentationId"> Optional: presentation ID. </param>
		public void PresentPresentationForPlan(string planId,
			Action<ProductViewResult, Plan> onResult, Action<bool> onContentLoaded = null,
			Action onCloseButtonClicked = null, string contentId = "", string presentationId = "")
		{
			if (string.IsNullOrEmpty(contentId))
				contentId = string.Empty;

			if (_implementation == null)
			{
				onResult(ProductViewResult.Cancelled, null);
				return;
			}

			_implementation.PresentPresentationForPlan(planId, onResult, onContentLoaded,
				onCloseButtonClicked, contentId, presentationId);
		}

		/// <summary>
		/// Setup a callback to intercept user actions in the native view.
		/// Make sure to call <see cref="ProcessPaywallAction"/> after executing your logic to go back to the native view.
		/// </summary>
		/// <param name="onAction"> Callback with the action options.
		/// If you don't handle every action, you HAVE TO call ProcessPaywallAction(true) otherwise
		/// the button will keep spinning and nothing will happen.</param>
		public void SetPaywallActionInterceptor(Action<PaywallAction> onAction)
		{
			_implementation?.SetPaywallActionInterceptor(onAction);
		}

		/// <summary>
		/// Call this after processing a <see cref="SetPaywallActionInterceptor"/> callback to go back to the native view.
		/// </summary>
		public void ProcessPaywallAction(bool process)
		{
			_implementation?.ProcessPaywallAction(process);
		}

		/// <summary>
		/// Restore purchases for a user.
		/// </summary>
		/// <param name="onSuccess"> Callback with the restored plan in case of success. </param>
		/// <param name="onError"> Callback with an error. </param>
		public void RestoreAllProducts(bool silent, Action<Plan> onSuccess, Action<string> onError)
		{
			_implementation?.RestoreAllProducts(silent, onSuccess, onError);
		}

		/// <summary>
		/// Retrieve the anonymous User ID generated by the SDK.
		/// </summary>
		public string GetAnonymousUserId()
		{
			return _implementation?.GetAnonymousUserId() ?? "";
		}

		/// <summary>
		/// Force the SDK to work with a specific language.
		/// </summary>
		/// <param name="language">ISO 639-1 or ISO 639-2 language code.</param>
		public void SetLanguage(string language)
		{
			_implementation.SetLanguage(language);
		}

		/// <summary>
		/// Sign the current Purchasely user out.
		/// </summary>
		public void UserLogout()
		{
			_implementation?.UserLogout();
		}

		/// <summary>
		/// Set the default paywall result callback.
		/// </summary>
		/// <param name="onResult"> Callback to be invoked after user action. </param>
		public void SetDefaultPresentationResultHandler(Action<ProductViewResult, Plan> onResult)
		{
			_implementation?.SetDefaultPresentationResultHandler(onResult);
		}

		/// <summary>
		///  Get data payload for a specific <see cref="Product"/>
		/// </summary>
		/// <param name="productId"> ID of the product from the developer console. </param>
		/// <param name="onSuccess"> Callback with the payload.</param>
		/// <param name="onError"> Callback with an error. </param>
		public void GetProduct(string productId, Action<Product> onSuccess, Action<string> onError)
		{
			if (_implementation == null)
			{
				onError("Purchasely is not supported on this platform.");
				return;
			}

			_implementation.ProductWithIdentifier(productId, onSuccess, onError);
		}

		/// <summary>
		/// Get data payload for a specific <see cref="Plan"/>
		/// </summary>
		/// <param name="planId"> ID of the plan from the developer console. </param>
		/// <param name="onSuccess"> Callback with the payload. </param>
		/// <param name="onError"> Callback with an error. </param>
		public void GetPlan(string planId, Action<Plan> onSuccess, Action<string> onError)
		{
			if (_implementation == null)
			{
				onError("Purchasely is not supported on this platform.");
				return;
			}

			_implementation.PlanWithIdentifier(planId, onSuccess, onError);
		}

		/// <summary>
		/// Get all of the available products.
		/// </summary>
		/// <param name="onSuccess"> Callback with the payload. </param>
		/// <param name="onError"> Callback with an error. </param>
		public void GetAllProducts(Action<List<Product>> onSuccess, Action<string> onError)
		{
			if (_implementation == null)
			{
				onError("Purchasely is not supported on this platform.");
				return;
			}

			_implementation.AllProducts(onSuccess, onError);
		}

		/// <summary>
		/// Purchase a plan without displaying UI.
		/// </summary>
		/// <param name="planId"> Plan to purchase. </param>
		/// <param name="onSuccess"> Callback with the payload after successful purchase.</param>
		/// <param name="onError"> Callback with an error. </param>
		/// <param name="contentId"> Optional: content ID. </param>
		public void Purchase(string planId,
			Action<Plan> onSuccess,
			Action<string> onError,
			string offerId = null,
			string contentId = null)
		{
			if (_implementation == null)
			{
				onError("Purchasely is not supported on this platform.");
				return;
			}

			_implementation.Purchase(planId,  onSuccess, onError, offerId, contentId);
		}

		/// <summary>
		/// Synchronize purchases with the Purchasely backend.
		/// </summary>
		public void Synchronize()
		{
			_implementation?.Synchronize();
		}

		/// <summary>
		/// Handle the deep link URL if your application was open via URL, and you have set up the deep link interception.
		/// </summary>
		public bool IsDeeplinkHandled(string url)
		{
			return _implementation?.IsDeeplinkHandled(url) ?? false;
		}

		/// <summary>
		/// Get the payload for the list of user subscriptions.
		/// </summary>
		/// <param name="onSuccess"> Callback with the payload. </param>
		/// <param name="onError"> Callback with an error. </param>
		public void GetUserSubscriptions(Action<List<SubscriptionData>> onSuccess, Action<string> onError)
		{
			if (_implementation == null)
			{
				onError("Purchasely is not supported on this platform.");
				return;
			}

			_implementation.GetUserSubscriptions(onSuccess, onError);
		}

		/// <summary>
		/// Show a native view with user subscriptions.
		/// </summary>
		public void PresentSubscriptions()
		{
			_implementation?.PresentSubscriptions();
		}

		/// <summary>
		/// Set an attribute for Purchasely.
		/// </summary>
		public void SetAttribute(PLYAttribute attribute, string value)
		{
			_implementation?.SetAttribute((int)attribute, value);
		}

		/// <summary>
		/// Set string user attribute for Purchasely.
		/// </summary>
		public void SetUserAttribute(string key, string value)
		{
			_implementation?.SetUserAttribute(key, value);
		}

		/// <summary>
		/// Set integer user attribute for Purchasely.
		/// </summary>
		public void SetUserAttribute(string key, int value)
		{
			_implementation?.SetUserAttribute(key, value);
		}

		/// <summary>
		/// Set floating value user attribute for Purchasely.
		/// </summary>
		public void SetUserAttribute(string key, float value)
		{
			_implementation?.SetUserAttribute(key, value);
		}

		/// <summary>
		/// Set boolean value user attribute for Purchasely.
		/// </summary>
		public void SetUserAttribute(string key, bool value)
		{
			_implementation?.SetUserAttribute(key, value);
		}

		/// <summary>
		/// Set date value user attribute for Purchasely.
		/// </summary>
		public void SetUserAttribute(string key, DateTime value)
		{
			_implementation?.SetUserAttribute(key, value);
		}

		/// <summary>
		/// Clear user attribute for Purchasely.
		/// </summary>
		public void ClearUserAttribute(string key)
		{
			_implementation?.ClearUserAttribute(key);
		}

		/// <summary>
		/// Clear all user attributes for Purchasely.
		/// </summary>
		public void ClearUserAttributes()
		{
			_implementation?.ClearUserAttributes();
		}

		/// <summary>
		/// Get user attribute for Purchasely.
		/// </summary>
		public string GetUserAttribute(string key)
		{
			return _implementation?.GetUserAttribute(key);
		}

		/// <summary>
		/// Send a call to SDK about the consumption of content.
		/// </summary>
		public void UserDidConsumeSubscriptionContent()
		{
			_implementation?.UserDidConsumeSubscriptionContent();
		}

		/// <summary>
		/// Fetch the native view content for a presentation.
		/// </summary>
		/// <param name="presentationId"> ID of the presentation to be fetched. </param>
		/// <param name="onSuccess"> Callback for the successful fetch. </param>
		/// <param name="onError"> Callback with error. </param>
		/// <param name="contentId"> Optional content ID. </param>
		public void FetchPresentation(string presentationId, Action<Presentation> onSuccess, Action<string> onError,
			string contentId = "")
		{
			_implementation?.FetchPresentation(presentationId, onSuccess, onError, contentId);
		}

		/// <summary>
		/// Fetch the native view content for a placement.
		/// </summary>
		/// <param name="placementId"> ID of the placement to be fetched. </param>
		/// <param name="onSuccess"> Callback for the successful fetch. </param>
		/// <param name="onError"> Callback with error. </param>
		/// <param name="contentId"> Optional content ID. </param>
		public void FetchPresentationForPlacement(string placementId, Action<Presentation> onSuccess,
			Action<string> onError, string contentId = "")
		{
			_implementation?.FetchPresentationForPlacement(placementId, onSuccess, onError, contentId);
		}

		/// <summary>
		/// Let SDK know that a presentation view has been shown in your app.
		/// </summary>
		/// <param name="presentation"> Presentation for which the content has been shown. </param>
		public void ClientPresentationOpened(Presentation presentation)
		{
			_implementation?.ClientPresentationOpened(presentation);
		}

		/// <summary>
		/// Let SDK know that a presentation view has been closed in your app.
		/// </summary>
		/// <param name="presentation"> Presentation for which the view has been closed. </param>
		public void ClientPresentationClosed(Presentation presentation)
		{
			_implementation?.ClientPresentationClosed(presentation);
		}

		/// <summary>
		/// Present content for previously fetched presentation.
		/// </summary>
		/// <param name="presentation"></param>
		/// <param name="onResult"> Callback to be invoked after user action. </param>
		/// <param name="onContentLoaded"> Optional: callback to be invoked when the content is loaded. </param>
		/// <param name="onCloseButtonClicked"> Optional: callback to be invoked when the user taps the close button. </param>
		public void PresentContentForPresentation(Presentation presentation, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded = null, Action onCloseButtonClicked = null)
		{
			_implementation?.PresentContentForPresentation(presentation, onResult, onContentLoaded, onCloseButtonClicked);
		}

		/// <summary>
		/// Sign promotional offer using StoreKit
		/// </summary>
		public void SignPromotionalOffer(string storeOfferId, string storeProductId, Action<PromotionalOfferSignature> onSuccess,
			Action<string> onError)
		{
			_implementation?.SignPromotionalOffer(storeOfferId, storeProductId, onSuccess, onError);
		}

		/// <summary>
		/// IsAnonymous
		/// </summary>
		public bool IsAnonymous()
		{
			return _implementation?.IsAnonymous() ?? false;
		}

		/// <summary>
		/// IsEligibileForIntroOffer
		/// </summary>
		public void IsEligibleForIntroOffer(string planVendorId, Action<bool> onSuccess, Action<string> onError)
		{
			_implementation?.IsEligibleForIntroOffer(planVendorId, onSuccess, onError);
		}

        /// <summary>
        /// Show Presentation
        /// </summary>
        public void ShowPresentation()
        {
            _implementation?.ShowPresentation();
        }

        /// <summary>
        /// Close Presentation
        /// </summary>
        public void ClosePresentation()
        {
            _implementation?.ClosePresentation();
        }

        /// <summary>
        /// hide Presentation
        /// </summary>
        public void HidePresentation()
        {
            _implementation?.HidePresentation();
        }

        /// <summary>
        /// Change SDK theme (light/dark/system)
        /// </summary>
        public void SetThemeMode(ThemeMode mode)
        {
	        _implementation?.SetThemeMode(mode);
        }
	}
}