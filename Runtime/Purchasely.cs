using System;
using System.Collections.Generic;
using UnityEngine;

namespace PurchaselyRuntime
{
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
		/// <param name="readyToPurchase"> Whether the application is ready to display the pay-wall. You can later change it. </param>
		/// <param name="logLevel"> Log level of the SDK. </param>
		/// <param name="runningMode"> Allows you to use Purchasely with another In-App purchase system to prepare a migration. More details here: https://docs.purchasely.com/quick-start-1/sdk-configuration.</param>
		/// <param name="onStartCompleted"> Callback received with the result of the SDK initialization. Boolean parameter represents the success status with an optional error.</param>
		/// <param name="onEventReceived"> Callback to be invoked when any events happen in the SDK. You should implement it at least to know when the purchase is successful.</param>
		/// <exception cref="ArgumentException"> Is thrown if the SDK is not configured in the Editor. In the Unity Editor go to Window->Purchasely, then provide your API key and other required data.</exception>
		public Purchasely(string userId, bool readyToPurchase, LogLevel logLevel, RunningMode runningMode,
			Action<bool, string> onStartCompleted, Action<Event> onEventReceived)
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

			_implementation.Init(settings.ApiKey, userId, readyToPurchase, (int) logLevel,
				(int) runningMode, onStartCompleted, onEventReceived);
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
		public void SetReadyToPurchase(bool ready)
		{
			_implementation?.SetIsReadyToPurchase(ready);
		}

		/// <summary>
		/// Show content for a specific placement.
		/// </summary>
		/// <param name="placementId"> Placement to show. </param>
		/// <param name="onResult"> Callback to be invoked after user action. </param>
		/// <param name="onContentLoaded"> Optional: callback to be invoked when the content is loaded. </param>
		/// <param name="onCloseButtonClicked"> Optional: callback to be invoked when the user taps the close button. </param>
		/// <param name="contentId"> Optional: content ID. </param>
		public void PresentContentForPlacement(string placementId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded = null, Action onCloseButtonClicked = null, string contentId = "")
		{
			if (string.IsNullOrEmpty(contentId))
				contentId = string.Empty;

			if (_implementation == null)
			{
				onResult(ProductViewResult.Cancelled, null);
				return;
			}

			_implementation.PresentContentForPlacement(placementId, onResult, onContentLoaded, onCloseButtonClicked,
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
		public void PresentContentForPresentation(string presentationId,
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

			_implementation.PresentContentForPresentation(presentationId, onResult, onContentLoaded,
				onCloseButtonClicked, contentId);
		}

		/// <summary>
		/// Show content for a specific presentation.
		/// </summary>
		/// <param name="productId"> Product to show. </param>
		/// <param name="onResult"> Callback to be invoked after user action. </param>
		/// <param name="onContentLoaded"> Optional: callback to be invoked when the content is loaded. </param>
		/// <param name="onCloseButtonClicked"> Optional: callback to be invoked when the user taps the close button. </param>
		/// <param name="contentId"> Optional: content ID. </param>
		/// <param name="presentationId"> Optional: presentation ID. </param>
		public void PresentContentForProduct(string productId,
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

			_implementation.PresentContentForProduct(productId, onResult, onContentLoaded,
				onCloseButtonClicked, contentId, presentationId);
		}

		/// <summary>
		/// Show content for a specific plan.
		/// </summary>
		/// <param name="planId"> Plan to show. </param>
		/// <param name="onResult"> Callback to be invoked after user action. </param>
		/// <param name="onContentLoaded"> Optional: callback to be invoked when the content is loaded. </param>
		/// <param name="onCloseButtonClicked"> Optional: callback to be invoked when the user taps the close button. </param>
		/// <param name="contentId"> Optional: content ID. </param>
		/// <param name="presentationId"> Optional: presentation ID. </param>
		public void PresentContentForPlan(string planId,
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

			_implementation.PresentContentForPlan(planId, onResult, onContentLoaded,
				onCloseButtonClicked, contentId, presentationId);
		}

		/// <summary>
		/// Setup a callback to intercept user actions in the native view.
		/// Make sure to call <see cref="ProcessPaywallAction"/> after executing your logic to go back to the native view.
		/// </summary>
		/// <param name="onActionJson"> Callback with the serialized action options.
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

		public void RestoreAllProducts(bool silent, Action<Plan> onSuccess, Action<string> onError)
		{
			_implementation?.RestoreAllProducts(silent, onSuccess, onError);
		}

		public string GetAnonymousUserId()
		{
			return _implementation?.GetAnonymousUserId() ?? "";
		}

		public void SetLanguage(string language)
		{
			_implementation.SetLanguage(language);
		}

		public void UserLogout()
		{
			_implementation?.UserLogout();
		}

		public void SetDefaultPresentationResultHandler(Action<ProductViewResult, Plan> onResult)
		{
			_implementation?.SetDefaultPresentationResultHandler(onResult);
		}

		public void GetProduct(string productId, Action<Product> onSuccess, Action<string> onError)
		{
			if (_implementation == null)
			{
				onError("Purchasely is not supported on this platform.");
				return;
			}

			_implementation.ProductWithIdentifier(productId, onSuccess, onError);
		}

		public void GetPlan(string planId, Action<Plan> onSuccess, Action<string> onError)
		{
			if (_implementation == null)
			{
				onError("Purchasely is not supported on this platform.");
				return;
			}

			_implementation.PlanWithIdentifier(planId, onSuccess, onError);
		}

		public void GetAllProducts(Action<List<Product>> onSuccess, Action<string> onError)
		{
			if (_implementation == null)
			{
				onError("Purchasely is not supported on this platform.");
				return;
			}

			_implementation.AllProducts(onSuccess, onError);
		}

		public void Purchase(string planId, Action<Plan> onSuccess, Action<string> onError,
			string contentId = "")
		{
			if (_implementation == null)
			{
				onError("Purchasely is not supported on this platform.");
				return;
			}

			_implementation.PurchaseWithPlanId(planId, onSuccess, onError);
		}

		public void HandleDeepLinkUrl(string url)
		{
			_implementation?.HandleDeepLinkUrl(url);
		}

		public void GetUserSubscriptions(Action<SubscriptionData> onSuccess, Action<string> onError)
		{
			if (_implementation == null)
			{
				onError("Purchasely is not supported on this platform.");
				return;
			}

			_implementation.GetUserSubscriptions(onSuccess, onError);
		}

		public void PresentSubscriptions()
		{
			_implementation?.PresentSubscriptions();
		}

		public void SetUserAttribute(string key, string value)
		{
			_implementation?.SetUserAttribute(key, value);
		}

		public void SetUserAttribute(string key, int value)
		{
			_implementation?.SetUserAttribute(key, value);
		}

		public void SetUserAttribute(string key, float value)
		{
			_implementation?.SetUserAttribute(key, value);
		}

		public void SetUserAttribute(string key, bool value)
		{
			_implementation?.SetUserAttribute(key, value);
		}

		public void SetUserAttribute(string key, DateTime value)
		{
			_implementation?.SetUserAttribute(key, value);
		}

		public void ClearUserAttribute(string key)
		{
			_implementation?.ClearUserAttribute(key);
		}

		public void ClearUserAttributes()
		{
			_implementation?.ClearUserAttributes();
		}

		public string GetUserAttribute(string key)
		{
			return _implementation?.GetUserAttribute(key);
		}

		public void UserDidConsumeSubscriptionContent()
		{
			_implementation?.UserDidConsumeSubscriptionContent();
		}
	}
}