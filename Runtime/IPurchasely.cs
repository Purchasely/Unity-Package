using System;
using System.Collections.Generic;

namespace PurchaselyRuntime
{
	internal interface IPurchasely
	{
		void Init(string apiKey, string userId, bool readyToOpenDeeplink, int logLevel, int runningMode,
			Action<bool, string> onStartCompleted, Action<Event> onEventReceived);

		void Init(string apiKey, string userId, bool readyToOpenDeeplink, int logLevel, int runningMode,
			bool storekit1, Action<bool, string> onStartCompleted);

		void UserLogin(string userId, Action<bool> onCompleted);

		void SetIsReadyToOpenDeeplink(bool ready);

		void PresentPresentationForPlacement(string placementId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId);

		void PresentPresentationWithId(string presentationId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId);

		void PresentPresentationForProduct(string productId, Action<ProductViewResult, Plan> onResult,
			Action<bool> onContentLoaded, Action onCloseButtonClicked, string contentId, string presentationId);

		void PresentPresentationForPlan(string planId, Action<ProductViewResult, Plan> onResult,
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

		void Purchase(string planId, Action<Plan> onSuccess,  Action<string> onError, string offerId,  string contentId);

		bool IsDeeplinkHandled(string url);

		void GetUserSubscriptions(Action<List<SubscriptionData>> onSuccess, Action<string> onError);

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

		void FetchPresentation(string presentationId, Action<Presentation> onSuccess, Action<string> onError,
			string contentId);

		void FetchPresentationForPlacement(string placementId, Action<Presentation> onSuccess, Action<string> onError,
			string contentId);

		void ClientPresentationOpened(Presentation presentation);

		void ClientPresentationClosed(Presentation presentation);

		void PresentContentForPresentation(Presentation presentation, Action<ProductViewResult, Plan> onResult, 
			Action<bool> onContentLoaded = null, Action onCloseButtonClicked = null);

		void SignPromotionalOffer(string storeOfferId, string storeProductId, Action<PromotionalOfferSignature> onSuccess,
			Action<string> onError);

		bool IsAnonymous();

		bool IsEligibleForIntroOffer(string planVendorId);
	}
}