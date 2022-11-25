using System;
using UnityEngine;

namespace Purchasely
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
			Action<bool, string> onStartCompleted, Action<PurchaselyEvent> onEventReceived)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			_implementation = new PurchaselyAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
			_implementation = new PurchaselyIos();
#endif
			var settings = Resources.Load<PurchaselySettings>(SettingsFullPath);
			if (settings == null)
				throw new ArgumentException(
					$"Purchasely settings were not found. Please, make sure that asset \'{SettingsFullPath}\' exists. In the Unity Editor go to Window->Purchasely, then provide your API key and other required data.");

			_implementation?.Init(settings.ApiKey, userId, readyToPurchase, (int) logLevel,
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
		/// Show pay wall for a specific placement.
		/// </summary>
		/// <param name="placementId"> Placement to show. </param>
		/// <param name="displayCloseButton"> Whether to display close button. </param>
		/// <param name="onResult"> Callback to be invoked after user action. </param>
		/// <param name="onContentLoaded"> Optional: callback to be invoked when the placement content is loaded. </param>
		/// <param name="onCloseButtonClicked"> Optional: callback to be invoked when the user taps the close button. </param>
		/// <param name="contentId"> Optional: content ID. </param>
		public void PresentContentForPlacement(string placementId, bool displayCloseButton, 
			Action<ProductViewResult, PurchaselyPlan> onResult, Action<bool> onContentLoaded = null, Action onCloseButtonClicked = null, 
			string contentId = "")
		{
			_implementation?.PresentContentForPlacement(placementId, displayCloseButton, onResult, onContentLoaded, 
				onCloseButtonClicked, contentId);
		}
	}
}