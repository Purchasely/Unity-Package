using System;
using System.Collections.Generic;

namespace PurchaselyRuntime
{
	internal interface IPurchasely
	{
		void Init(string apiKey, string userId, bool readyToPurchase, int logLevel, int runningMode,
			Action<bool, string> onStartCompleted, Action<Event> onEventReceived);

		void UserLogin(string userId, Action<bool> onCompleted);

		void SetIsReadyToPurchase(bool ready);

		void PresentContentForPlacement(string placementId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId);

		void PresentContentForPresentation(string presentationId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId);

		void PresentContentForProduct(string productId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId, string presentationId);

		void PresentContentForPlan(string planId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId, string presentationId);

		void SetPaywallActionInterceptor(Action<PaywallAction> onAction);
		void ProcessPaywallAction(bool process);

		void RestoreAllProducts(bool silent, Action<Plan> onSuccess, Action<string> onError);

		string GetAnonymousUserId();

		void SetLanguage(string language);

		void UserLogout();

		void SetDefaultPresentationResultHandler(Action<ProductViewResult, Plan> onResult);

		void ProductWithIdentifier(string productId, Action<Product> onSuccess, Action<string> onError);

		void PlanWithIdentifier(string planId, Action<Plan> onSuccess, Action<string> onError);

		void AllProducts(Action<List<Product>> onSuccess, Action<string> onError);

		void PurchaseWithPlanId(string planId, Action<Plan> onSuccess, Action<string> onError,
			string contentId);

		void HandleDeepLinkUrl(string url);

		void GetUserSubscriptions(Action<SubscriptionData> onSuccess, Action<string> onError);

		void PresentSubscriptions();

		void SetUserAttribute(string key, string value);

		void SetUserAttribute(string key, int value);

		void SetUserAttribute(string key, float value);

		void SetUserAttribute(string key, bool value);

		void SetUserAttribute(string key, DateTime value);

		void ClearUserAttribute(string key);

		void ClearUserAttributes();

		string GetUserAttribute(string key);

		void UserDidConsumeSubscriptionContent();
	}
}